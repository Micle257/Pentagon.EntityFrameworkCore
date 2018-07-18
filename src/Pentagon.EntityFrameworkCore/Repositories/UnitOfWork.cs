﻿// -----------------------------------------------------------------------
//  <copyright file="UnitOfWork.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    /// <summary> Represents an unit of work that communicate with a database and manage repository changes to the database. </summary>
    /// <typeparam name="TContext"> The type of the db context. </typeparam>
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
            where TContext : class, IApplicationContext
    {
        /// <summary> The repository factory. </summary>
        [NotNull]
        readonly IRepositoryFactory _repositoryFactory;

        /// <summary> The update service. </summary>
        [NotNull]
        readonly IDbContextUpdateService _updateService;

        /// <summary> The update service. </summary>
        [NotNull]
        readonly IDbContextDeleteService _deleteService;

        /// <summary> The identity service. </summary>
        [NotNull]
        readonly IDbContextIdentityService _identityService;

        readonly IDatabaseCommitManager _commitManager;

        /// <summary> The database context. </summary>
        readonly DbContext _dbContext;

        List<Entry> _entries = new List<Entry>();

        /// <summary> Initializes a new instance of the <see cref="UnitOfWork{TContext}" /> class. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="repositoryFactory"> The repository factory. </param>
        /// <param name="updateService"> The update service. </param>
        /// <param name="deleteService"> The delete service. </param>
        /// <param name="identityService"> The identity service. </param>
        /// <param name="commitManager"> The commit manager. </param>
        public UnitOfWork([NotNull] TContext context,
                          [NotNull] IRepositoryFactory repositoryFactory,
                          [NotNull] IDbContextUpdateService updateService,
                          [NotNull] IDbContextDeleteService deleteService,
                          [NotNull] IDbContextIdentityService identityService,
                          [NotNull] IDatabaseCommitManager commitManager)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            Require.IsType(() => context, out _dbContext);
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _commitManager = commitManager ?? throw new ArgumentNullException(nameof(commitManager));
            Context = context;

            _dbContext.ChangeTracker.StateChanged += OnStateChanged;
        }

        void OnStateChanged(object sender, EntityStateChangedEventArgs entityStateChangedEventArgs)
        {
            ResetEntries();
        }

        void ResetEntries()
        {
            _entries = _dbContext.ChangeTracker
                                 .Entries()
                                 .Select(a => new Entry(a.Entity as IEntity, a.State.ToEntityStateType(), null))
                                 .ToList();
        }

        /// <inheritdoc />
        public TContext Context { get; }

        /// <inheritdoc />
        public virtual IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new()
        {
            // get repository from factory
            var repo = _repositoryFactory.GetRepository<TEntity>(Context);

            // attach event delegate to new repository
            repo.Commiting += OnCommiting;

            return repo;
        }

        /// <inheritdoc />
        public int Commit()
        {
            if (!CommitCore())
                return 0;

            return _dbContext.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<int> CommitAsync()
        {
            if (!CommitCore())
                return 0;

            var saveResult = await _dbContext.SaveChangesAsync();
            
            _commitManager.RaiseCommited(typeof(TContext), _entries);

            return saveResult;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        /// <param name="disposing"> The disposing. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Context.Dispose();
        }

        void OnCommiting(object sender, CommitEventArgs commitEventArgs)
        {
            foreach (var entry in commitEventArgs.Entries)
            {
                if (entry.UserId == null)
                    continue;
            }

            var repo = sender.GetType().GenericTypeArguments.FirstOrDefault();

            _commitManager?.RaiseCommiting(typeof(TContext), repo, commitEventArgs.Entries);
        }

        bool CommitCore()
        {
            if (!_dbContext.ChangeTracker.HasChanges())
                return false;

            _updateService.Apply(Context);
            _deleteService.Apply(Context, Context.HasHardDeleteBehavior);
            _identityService.Apply(Context, EntryMap);

            return true;
        }
    }
}