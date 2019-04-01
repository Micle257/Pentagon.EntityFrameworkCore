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
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;

    public class ConcurrencyConflictResolver : IConcurrencyConflictResolver
    {
        public async Task<ConcurrencyConflictResolveResult> ResolveAsync(IApplicationContext appContext, Func<IApplicationContext> contextFactory)
        {
            if (appContext == null)
                throw new ArgumentNullException(nameof(appContext));

            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            var dbContext = appContext.GetDbContext();
            
            // get all entries tracked by EF
            var localEntities = dbContext.ChangeTracker?.Entries()
                                         // Filter only valid IEntity objects that implement concurrency support and are modified
                                         .Where(e => e.Entity is IEntity && e.Entity is IConcurrencyStampSupport && e.State == EntityState.Modified)
                                         .Select(a => a.Entity as IEntity)
                                         .ToList();

            var remoteEntries = new List<IEntity>();

            using (var context = contextFactory().GetDbContext())
            {
                foreach (var entity in localEntities)
                {
                    // for ensured data that are not in local EF cache, we create new UoW and get the entity version in database
                    var e = await context.FindAsync(entity.GetType(), entity.Id) as IEntity;

                    remoteEntries.Add(e);
                }
            }

            // tie together remote and local entities
            var concat = localEntities.Zip(remoteEntries, (local, remote) => (Local: local, Database: remote));

            var conflicts = new List<ConcurrencyConflictPair>();

            foreach (var (local, database) in concat)
            {
                // check if both compared entities supports concurrency checks
                if (!(local is IConcurrencyStampSupport lc && database is IConcurrencyStampSupport rc))
                {
                    return new ConcurrencyConflictResolveResult
                           {
                                   CanBeDetermine = false
                           };
                }

                // if the concurrency ids are not equal...
                if (!lc.ConcurrencyStamp.Equals(rc.ConcurrencyStamp))
                {
                    conflicts.Add(new ConcurrencyConflictPair
                                  {
                                          Local = new ConcurrencyConflictEntity { Entity = local },
                                          Remote = new ConcurrencyConflictEntity { Entity = database }
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