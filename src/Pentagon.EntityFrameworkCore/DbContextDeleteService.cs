// -----------------------------------------------------------------------
//  <copyright file="DbContextDeleteService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    using System;
    using System.Linq;
    using Abstractions;
    using Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;
    using Pentagon.Extensions.DependencyInjection;

    /// <summary>
    /// Represents an implementation of <see cref="IDbContextDeleteService"/> for entity framework core.
    /// </summary>
    [Register(RegisterType.Singleton, typeof(IDbContextDeleteService))]
    public class DbContextDeleteService : IDbContextDeleteService
    {
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

                    if (entry.Entity is IDeleteTimeStampSupport entityTimed)
                        entityTimed.DeletedAt = DateTimeOffset.Now;

                    entry.State = EntityState.Unchanged;
                }
            }
        }
    }
}