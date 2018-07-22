// -----------------------------------------------------------------------
//  <copyright file="DbContextIdentityService.cs">
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

    /// <summary> Represents an implementation of <see cref="IDbContextIdentityService" /> for entity framework core. </summary>
    /// <seealso cref="IDbContextIdentityService" />
    public class DbContextIdentityService : IDbContextIdentityService
    {
        /// <inheritdoc />
        public void Apply(IApplicationContext appContext, object userId)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(appContext is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            if (!dbContext.ChangeTracker.HasChanges())
                return;

            var entries = dbContext.ChangeTracker?.Entries()
                                   .Where(e => e.Entity is IEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                if (entry.Entity is ITimeStampIdentitySupport entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.CreatedBy = userId;
                            break;

                        case EntityState.Modified:
                            entity.UpdatedBy = userId;
                            break;
                    }
                }

                if (entry.Entity is IDeleteTimeStampIdentitySupport deleteEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            deleteEntity.DeletedBy = userId;
                            break;
                    }
                }
            }
        }
    }
}