namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;

    public class UnitOfWorkCommitExecutor<TContext>
            where TContext : IApplicationContext
    {
        [NotNull]
        readonly IConcurrencyConflictResolver<TContext> _conflictResolver;

        [NotNull]
        readonly IDbContextUpdateService _updateService;

        [NotNull]
        readonly IDbContextDeleteService _deleteService;

        [NotNull]
        readonly IDbContextIdentityService _identityService;

        [NotNull]
        readonly IDatabaseCommitManager _commitManager;

        public UnitOfWorkCommitExecutor([NotNull] IConcurrencyConflictResolver<TContext> conflictResolver,
                                        [NotNull] IDbContextUpdateService updateService,
                                        [NotNull] IDbContextDeleteService deleteService,
                                        [NotNull] IDbContextIdentityService identityService,
                                        [NotNull] IDatabaseCommitManager commitManager)
        {
            _conflictResolver = conflictResolver;
            _updateService = updateService;
            _deleteService = deleteService;
            _identityService = identityService;
            _commitManager = commitManager;
        }

        public async Task<bool> ExecuteCommitAsync(IUnitOfWork<TContext> unitOfWork)
        {
            var _dbContext = unitOfWork.Context as DbContext;

            try
            {
                _dbContext.ChangeTracker.DetectChanges();

                if (!_dbContext.ChangeTracker.HasChanges())
                    return false;

                var conflictResult = await _conflictResolver.ResolveAsync(unitOfWork.Context);

                _updateService.Apply(unitOfWork.Context);
                _deleteService.Apply(unitOfWork.Context, unitOfWork.Context.HasHardDeleteBehavior);
                _identityService.Apply(unitOfWork.Context, unitOfWork.UserId);

                // save the database without appling changes
                await _dbContext.SaveChangesAsync(false).ConfigureAwait(false);

                // raise all changes
                _commitManager.RaiseCommited(typeof(TContext), GetEntries(_dbContext));

                // accept changes
                _dbContext.ChangeTracker.AcceptAllChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        IEnumerable<Entry> GetEntries(DbContext _dbContext)
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                yield return new Entry(entry.Entity as IEntity, entry.State.ToEntityStateType());
            }
        }
    }

    public class ConcurrencyConflictResolver<TContext> : IConcurrencyConflictResolver<TContext>
            where TContext : IApplicationContext
    {
        readonly IUnitOfWorkFactory<TContext> _unitOfWorkFactory;

        public ConcurrencyConflictResolver(IUnitOfWorkFactory<TContext> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ConcurrencyConflictResolveResult> ResolveAsync(IApplicationContext appContext)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(appContext is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            var localEntities = dbContext.ChangeTracker?.Entries()
                                         .Where(e => e.Entity is IEntity && (e.State == EntityState.Modified))
                                         .Select(a => a.Entity as IEntity)
                                         .ToList();

            var remoteEntries = new List<IEntity>();

            foreach (var entity in localEntities)
            {
                var e = await (_unitOfWorkFactory.Create().Context as DbContext).FindAsync(entity.GetType(), entity.Id) as IEntity;

                remoteEntries.Add(e);
            }

            // tie together remote and local entities
            var concat = localEntities.Zip(remoteEntries, (local, remote) => (local, remote));

            var conflicts = new List<IEntity>();

            foreach (var t in concat)
            {
                // check if both compared enties supports concurrency checks
                if (t.local is IConcurrencyStampSupport lc && t.remote is IConcurrencyStampSupport rc)
                {
                    // if the concurrency ids are not equal...
                    if (!lc.ConcurrencyStamp.Equals(rc.ConcurrencyStamp))
                    {
                        conflicts.Add(t.local);
                    }
                }
            }

            return new ConcurrencyConflictResolveResult
            {
                ConflictedEntities = conflicts
            };
        }
    }
}