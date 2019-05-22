// -----------------------------------------------------------------------
//  <copyright file="ApplicationContext.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using Extensions;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public abstract class BaseApplicationContext : DbContext, IApplicationContext
    {
        [NotNull]
        readonly ILogger _logger;

        readonly Lazy<IConcurrencyConflictResolver> _conflictResolver = new Lazy<IConcurrencyConflictResolver>(() => new ConcurrencyConflictResolver());

        readonly HashSet<string> _supportedProviders = new HashSet<string>
                                                       {
                                                               "Microsoft.EntityFrameworkCore.SqlServer",
                                                               //"Microsoft.EntityFrameworkCore.Sqlite",
                                                               //"Npgsql.EntityFrameworkCore.PostgreSQL",
                                                               //"Pomelo.EntityFrameworkCore.MySql",
                                                               "Microsoft.EntityFrameworkCore.InMemory"
                                                       };

        readonly bool _isInitialized;

        readonly IDbContextUpdateService _updateService;

        readonly IDbContextDeleteService _deleteService;

        protected BaseApplicationContext([NotNull] ILogger logger,
                                         [NotNull] IDbContextUpdateService updateService,
                                         [NotNull] IDbContextDeleteService deleteService,
                                         DbContextOptions options) : base(options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _isInitialized = true;

            ChangeTracker.StateChanged += OnStateChanged;
            ChangeTracker.Tracked += OnTracked;
        }

        protected BaseApplicationContext(DbContextOptions options) : base(options)
        {
            _logger = NullLogger.Instance;

            ChangeTracker.StateChanged += OnStateChanged;
            ChangeTracker.Tracked += OnTracked;
        }

        protected BaseApplicationContext()
        {
            _logger = NullLogger.Instance;

            ChangeTracker.StateChanged += OnStateChanged;
            ChangeTracker.Tracked += OnTracked;
        }

        /// <inheritdoc />
        public event EventHandler<CommitEventArgs> Commiting;

        /// <inheritdoc />
        public bool UseTimeSourceFromEntities { get; set; }

        public bool AutoResolveConflictsFromSameUser { get; set; }

        protected virtual IModelConfiguration ModelConfiguration { get; } = new SqlServerModelConfiguration();

        /// <inheritdoc />
        public IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new() => new Repository<TEntity>(Set<TEntity>());

        /// <inheritdoc />
        public Task<UnitOfWorkCommitResult> ExecuteCommitAsync(CancellationToken cancellationToken = default)
        {
            return CommitCoreAsync(async (db, ct) => await db.SaveChangesAsync(false, ct).ConfigureAwait(false), cancellationToken);
        }

        /// <inheritdoc />
        public UnitOfWorkCommitResult ExecuteCommit()
        {
            return CommitCoreAsync((db, ct) => Task.FromResult(db.SaveChanges(false))).Result;
        }

        protected virtual void OnModelCreatingCore(ModelBuilder modelBuilder) { }

        protected virtual void SetupCoreModel([NotNull] ModelBuilder modelBuilder)
        {
            ModelConfiguration?.SetupModel(modelBuilder, Database.ProviderName);
        }

        /// <inheritdoc />
        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!_supportedProviders.Contains(Database.ProviderName))
                _logger.LogWarning($"Provider ({Database.ProviderName}) is not fully supported by model. Custom operations might be needed.");

            OnModelCreatingCore(modelBuilder);

            SetupCoreModel(modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder)));
        }

        async Task<UnitOfWorkCommitResult> CommitCoreAsync([NotNull] Func<DbContext, CancellationToken, Task<int>> callback, CancellationToken cancellationToken = default)
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"Dependencies for this instance ({GetType().Name}) are not initialized.");

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                ChangeTracker.DetectChanges();

                if (!ChangeTracker.HasChanges())
                    return new UnitOfWorkCommitResult();

                _updateService.Apply(this, UseTimeSourceFromEntities);
                _deleteService.Apply(this, UseTimeSourceFromEntities);

                var conflictResult = await _conflictResolver.Value.ResolveAsync(this, () => (IApplicationContext) Activator.CreateInstance(GetType())).ConfigureAwait(false);

                var conflictPairs = new List<ConcurrencyConflictPair>();

                if (conflictResult.CanBeDetermine && conflictResult.HasConflicts)
                {
                    conflictPairs = conflictResult.ConflictedEntities.ToList();

                    if (AutoResolveConflictsFromSameUser)
                    {
                        var userConflicts = conflictResult.ConflictedEntities
                                                          .Where(a => a.Local.UpdatedUserId != null && a.Remote.UpdatedUserId != null)
                                                          .Where(a => a.Local.UpdatedAt != null && a.Remote.UpdatedAt != null)
                                                          .Where(a => a.Local.UpdatedUserId.Equals(a.Remote.UpdatedUserId));

                        foreach (var userConflict in userConflicts)
                        {
                            Debug.Assert(userConflict.Local.UpdatedAt != null, "userConflict.Local.UpdatedAt != null");
                            Debug.Assert(userConflict.Remote.UpdatedAt != null, "userConflict.Remote.UpdatedAt != null");

                            var isLocalNewer = userConflict.Local.UpdatedAt.Value > userConflict.Remote.UpdatedAt.Value;

                            if (isLocalNewer)
                            {
                                conflictPairs.Remove(userConflict);
                                //Entry(userConflict.Local.Entity).State = EntityState.Modified;
                            }
                        }
                    }

                    // remove conflicted entities from change tracker
                    foreach (var entityEntry in conflictPairs.Select(a => Entry(a.Local.Entity)))
                        entityEntry.State = EntityState.Detached;
                }

                // save the database without applying changes
                var result = await callback(this, cancellationToken);

                // accept changes
                ChangeTracker.AcceptAllChanges();

                return new UnitOfWorkCommitResult
                       {
                               CommitResult = result,
                               Conflicts = conflictPairs
                       };
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError(message: "Unexpected concurrency error from SaveChanges method.");

                return new UnitOfWorkCommitResult {Exception = e};
            }
            catch (DbUpdateException e)
            {
                _logger.LogError($"Database update error occured. ({e.Message})");

                return new UnitOfWorkCommitResult {Exception = e};
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error occured while committing database context. ({e.Message})");

                return new UnitOfWorkCommitResult {Exception = e};
            }
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
            Commiting?.Invoke(this, new CommitEventArgs(commitEventArgs?.Entries.ToArray()));
        }
    }
}