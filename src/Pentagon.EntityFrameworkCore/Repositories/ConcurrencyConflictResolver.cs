namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;

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