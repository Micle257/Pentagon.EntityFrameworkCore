// -----------------------------------------------------------------------
//  <copyright file="DbContextDeleteService.cs">
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
    using Microsoft.Extensions.Logging;

    /// <summary> Represents an implementation of <see cref="IDbContextDeleteService" /> for entity framework core. </summary>
    public class DbContextDeleteService : IDbContextDeleteService
    {
        readonly ILogger<DbContextDeleteService> _logger;

        public DbContextDeleteService(ILogger<DbContextDeleteService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void Apply(IApplicationContext appContext, bool? isHardDelete = null)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(appContext is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            if (!dbContext.ChangeTracker.HasChanges())
                return;

            var entries = dbContext.ChangeTracker?.Entries()
                                   .Where(e => e.Entity is IEntity && e.State == EntityState.Deleted);

            var hardDelete = isHardDelete.HasValue ? isHardDelete.Value : appContext.HasHardDeleteBehavior;

            foreach (var entry in entries)
            {
                if (!hardDelete)
                {
                    if (entry.Entity is IDeletedFlagSupport entity)
                        entity.IsDeletedFlag = true;
                    else
                    {
                        _logger.LogWarning("The database context is marked for soft deletion, but no IsDeletedFlag is available.");
                        continue;
                    }

                    if (entry.Entity is IDeleteTimeStampSupport entityTimed)
                        entityTimed.DeletedAt = DateTimeOffset.Now;

                    entry.State = EntityState.Unchanged;
                }
            }
        }
    }
}