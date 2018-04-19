// -----------------------------------------------------------------------
//  <copyright file="DbContextUpdateService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using Abstractions;
    using Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;
    using Pentagon.Extensions.DependencyInjection;

    /// <summary>
    /// Represents an implementation of <see cref="IDbContextUpdateService"/> for entity framework core.
    /// </summary>
    [Register(RegisterType.Singleton, typeof(IDbContextUpdateService))]
    public class DbContextUpdateService : IDbContextUpdateService
    {
        /// <inheritdoc />
        public void Apply(IApplicationContext appContext)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(appContext is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            if (!dbContext.ChangeTracker.HasChanges())
                return;

            var entries = dbContext.ChangeTracker?.Entries()
                                   .Where(e => e.Entity is IEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is ITimeStampSupport entityTimed)
                        entityTimed.CreatedAt = DateTimeOffset.Now;

                    if (entry.Entity is ICreateStampSupport createStamp)
                        createStamp.CreateGuid = Guid.NewGuid();
                }

                if (entry.Entity is ITimeStampSupport entityTimed2)
                    entityTimed2.LastUpdatedAt = DateTimeOffset.Now;

                if (entry.Entity is IConcurrencyStampSupport concurrency)
                    concurrency.ConcurrencyStamp = Guid.NewGuid();
            }
        }
    }
}