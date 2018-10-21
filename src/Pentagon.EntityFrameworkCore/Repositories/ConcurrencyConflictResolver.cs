// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictResolver.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

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
        readonly IContextFactory<TContext> _unitOfWorkFactory;

        public ConcurrencyConflictResolver(IContextFactory<TContext> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ConcurrencyConflictResolveResult> ResolveAsync(IApplicationContext appContext)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(appContext is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            // get all entries tracked by EF
            var localEntities = dbContext.ChangeTracker?.Entries()
                                         // Filter only valid IEntity objects that implement concurrency support and are modified
                                         .Where(e => e.Entity is IEntity && e.Entity is IConcurrencyStampSupport && e.State == EntityState.Modified)
                                         .Select(a => a.Entity as IEntity)
                                         .ToList();

            var remoteEntries = new List<IEntity>();

            foreach (var entity in localEntities)
            {
                // for ensured data that are not in local EF cache, we create new UoW and get the entity version in database
                var e = await (_unitOfWorkFactory.CreateContext() as DbContext).FindAsync(entity.GetType(), entity.Id) as IEntity;

                remoteEntries.Add(e);
            }

            // tie together remote and local entities
            var concat = localEntities.Zip(remoteEntries, (local, remote) => (Local: local, Database: remote));

            var conflicts = new List<ConcurrencyConflictPair>();

            foreach (var t in concat)
            {
                // check if both compared enties supports concurrency checks
                if (t.Local is IConcurrencyStampSupport lc && t.Database is IConcurrencyStampSupport rc)
                {
                    // if the concurrency ids are not equal...
                    if (!lc.ConcurrencyStamp.Equals(rc.ConcurrencyStamp))
                        conflicts.Add(new ConcurrencyConflictPair
                                      {
                                              Posted = new ConcurrencyConflictEntity { Entity = t.Local },
                                              FromDatabase = new ConcurrencyConflictEntity { Entity = t.Database }
                                      });
                }
            }

            return new ConcurrencyConflictResolveResult
                   {
                           ConflictedEntities = conflicts
                   };
        }
    }
}