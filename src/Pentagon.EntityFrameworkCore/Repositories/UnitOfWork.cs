// -----------------------------------------------------------------------
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
        [NotNull]
        readonly IConcurrencyConflictResolver<TContext> _conflictResolver;

        /// <summary> The database context. </summary>
        readonly DbContext _dbContext;

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
                          [NotNull] IDatabaseCommitManager commitManager,
                          [NotNull] IConcurrencyConflictResolver<TContext> conflictResolver)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Require.IsType(() => context, out _dbContext);

            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _commitManager = commitManager ?? throw new ArgumentNullException(nameof(commitManager));
            _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));

            Context = context;

            _dbContext.ChangeTracker.StateChanged += OnStateChanged;
            _dbContext.ChangeTracker.Tracked += OnTracked;
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

        /// <inheritdoc />
        public TContext Context { get; }

        /// <inheritdoc />
        public bool IsUserAttached => UserId != null;

        /// <inheritdoc />
        public object UserId { get; set; }

        /// <inheritdoc />
        public virtual IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new()
        {
            // get repository from factory
            var repo = _repositoryFactory.GetRepository<TEntity>(Context);

            return repo;
        }

        /// <inheritdoc />
        public bool Commit()
        {
            return CommitCore(() =>
                              {
                                  _dbContext.SaveChanges(false);
                                  return Task.CompletedTask;
                              }).Result;
        }

        IEnumerable<Entry> GetEntries()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                yield return new Entry(entry.Entity as IEntity, entry.State.ToEntityStateType());
            }
        }

        /// <inheritdoc />
        public Task<bool> CommitAsync()
        {
            return CommitCore(async () => await _dbContext.SaveChangesAsync(false).ConfigureAwait(false));
        }

        async Task<bool> CommitCore(Func<Task> saveCallback)
        {
            try
            {
                _dbContext.ChangeTracker.DetectChanges();

                if (!_dbContext.ChangeTracker.HasChanges())
                    return false;



                _updateService.Apply(Context);
                _deleteService.Apply(Context, Context.HasHardDeleteBehavior);
                _identityService.Apply(Context, UserId);

                // save the database without appling changes
                await saveCallback().ConfigureAwait(false);

                // raise all changes
                _commitManager.RaiseCommited(typeof(TContext), GetEntries());

                // accept changes
                _dbContext.ChangeTracker.AcceptAllChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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

        void OnCommiting(CommitEventArgs commitEventArgs)
        {
            _commitManager?.RaiseCommiting(typeof(TContext), commitEventArgs.Entries.FirstOrDefault().Entity.GetType(), commitEventArgs.Entries);
        }
    }
}