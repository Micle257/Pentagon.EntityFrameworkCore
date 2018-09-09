// -----------------------------------------------------------------------
//  <copyright file="UnitOfWork.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Linq;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    
    /// <summary> Represents an unit of work that communicate with a database and manage repository changes to the database. </summary>
    /// /// <typeparam name="TContext"> The type of the db context. </typeparam>
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
            where TContext : class, IApplicationContext
    {
        /// <summary> The repository factory. </summary>
        [NotNull]
        readonly IRepositoryFactory _repositoryFactory;

        [NotNull]
        readonly IDatabaseCommitManager _commitManager;
        
        /// <inheritdoc />
        public TContext Context { get; }

        /// <inheritdoc />
        public bool UseTimeSourceFromEntities { get; set; }

        /// <inheritdoc />
        IApplicationContext IUnitOfWork.Context => Context;

        /// <summary> The database context. </summary>
        readonly DbContext _dbContext;

        TContext _context;

        /// <summary> Initializes a new instance of the <see cref="UnitOfWork{TContext}" /> class. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="repositoryFactory"> The repository factory. </param>
        /// <param name="commitManager"> The commit manager. </param>
        public UnitOfWork([NotNull] TContext context,
                          [NotNull] IRepositoryFactory repositoryFactory,
                          [NotNull] IDatabaseCommitManager commitManager)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Require.IsType(() => context, out _dbContext);

            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _commitManager = commitManager ?? throw new ArgumentNullException(nameof(commitManager));

            Context = context;

            _dbContext.ChangeTracker.StateChanged += OnStateChanged;
            _dbContext.ChangeTracker.Tracked += OnTracked;
        }
        
        /// <inheritdoc />
        public virtual IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new()
        {
            // get repository from factory
            var repo = _repositoryFactory.GetRepository<TEntity>(Context);

            return repo;
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

        void OnTracked(object sender, EntityTrackedEventArgs args)
        {
            var entity = args.Entry.Entity as IEntity;

            // entity has been tracked (get, add ...), commited is like added (for UI change)
            OnCommiting(new CommitEventArgs(new Entry(entity, EntityStateType.Added)));
        }

        void OnStateChanged(object sender, EntityStateChangedEventArgs args)
        {
            var entity = args.Entry.Entity as IEntity;
            var state = args.NewState.ToEntityStateType();
            var oldState = args.OldState;

            // if state has not changed for the API, cancel
            if (state == 0)
                return;

            // if the change was from unchanged to added
            // cancel call, because it is adding entity scenario and it would end up in two event fires
            if (oldState == EntityState.Unchanged && state == EntityStateType.Added)
                return;

            OnCommiting(new CommitEventArgs(new Entry(entity, state)));
        }

        void OnCommiting(CommitEventArgs commitEventArgs)
        {
            _commitManager?.RaiseCommiting(Context.GetType(), commitEventArgs.Entries.FirstOrDefault().Entity.GetType(), commitEventArgs.Entries);
        }
    }
}