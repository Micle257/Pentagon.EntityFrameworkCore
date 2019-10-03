// -----------------------------------------------------------------------
//  <copyright file="DbContextDeleteService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using Interfaces;
    using Interfaces.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary> Represents an implementation of <see cref="IDbContextChangeService" /> for entity framework core. </summary>
    public class DbContextChangeService : IDbContextChangeService
    {
        readonly ILogger<DbContextChangeService> _logger;
        readonly IDataUserProvider _userProvider;

        public DbContextChangeService(ILogger<DbContextChangeService> logger, IDataUserProvider userProvider)
        {
            _logger = logger;
            _userProvider = userProvider;
        }

        /// <inheritdoc />
        public void ApplyUpdate(IApplicationContext appContext, bool useTimestampFromEntity)
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
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is ICreateTimeStampSupport entityTimed)
                    {
                        entityTimed.CreatedAt = useTimestampFromEntity
                                                        ? (entityTimed.CreatedAt == default
                                                                   ? DateTimeOffset.Now
                                                                   : entityTimed.CreatedAt)
                                                        : DateTimeOffset.Now;
                    }

                    if (entry.Entity is ICreateStampSupport createStamp)
                        createStamp.Uuid = Guid.NewGuid();

                    if (entry.Entity is ICreateTimeStampIdentitySupport identity)
                        identity.CreatedUserId = _userProvider.UserId;

                    if (entry.Entity is ICreatedUserEntitySupport identityName)
                        identityName.CreatedUser = _userProvider.UserName;
                }

                // set last updated at when the entity has modified
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IUpdateTimeStampSupport entityTimed2)
                    {
                        entityTimed2.UpdatedAt = useTimestampFromEntity
                                                        ? (entityTimed2.UpdatedAt.HasValue
                                                                   ? DateTimeOffset.Now
                                                                   : entityTimed2.UpdatedAt)
                                                        : DateTimeOffset.Now;
                    }

                    if (entry.Entity is IUpdateTimeStampIdentitySupport identity)
                        identity.UpdatedUserId = _userProvider.UserId;

                    if (entry.Entity is IUpdatedUserEntitySupport identityName)
                        identityName.UpdatedUser = _userProvider.UserName;
                }
            }
        }

        /// <inheritdoc />
        public void ApplyConcurrency(IApplicationContext appContext)
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
                // generate new concurrency id both for add and update
                if (entry.Entity is IConcurrencyStampSupport concurrency)
                    concurrency.ConcurrencyStamp = Guid.NewGuid();
            }
        }

        /// <inheritdoc />
        public void ApplyDelete(IApplicationContext appContext, bool useTimestampFromEntity)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(appContext is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            if (!dbContext.ChangeTracker.HasChanges())
                return;

            var entries = dbContext.ChangeTracker?.Entries()
                                   .Where(e => e.Entity is IEntity && (e.State == EntityState.Deleted || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var hardDelete = !(entry.Entity is IDeletedFlagSupport);

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IDeletedFlagSupport entity && entity.DeletedFlag)
                    {
                        if (entry.Entity is IDeleteTimeStampSupport entityTimed)
                        {
                            entityTimed.DeletedAt = useTimestampFromEntity
                                                            ? (entityTimed.DeletedAt.HasValue
                                                                       ? DateTimeOffset.Now
                                                                       : entityTimed.DeletedAt)
                                                            : DateTimeOffset.Now;
                        }

                        if (entry.Entity is IDeleteTimeStampIdentitySupport deleteEntity)
                            deleteEntity.DeletedUserId = _userProvider.UserId;

                        if (entry.Entity is IDeletedUserEntitySupport identityName)
                            identityName.DeletedUser = _userProvider.UserName;
                    }
                }
                else if (!hardDelete)
                {
                    ((IDeletedFlagSupport)entry.Entity).DeletedFlag = true;

                    entry.State = EntityState.Modified;

                    if (entry.Entity is IDeleteTimeStampSupport entityTimed)
                    {
                        entityTimed.DeletedAt = useTimestampFromEntity
                                                        ? (entityTimed.DeletedAt.HasValue
                                                                   ? DateTimeOffset.Now
                                                                   : entityTimed.DeletedAt)
                                                        : DateTimeOffset.Now;
                    }

                    if (entry.Entity is IDeleteTimeStampIdentitySupport deleteEntity)
                        deleteEntity.DeletedUserId = _userProvider.UserId;

                    if (entry.Entity is IDeletedUserEntitySupport identityName)
                        identityName.DeletedUser = _userProvider.UserName;
                }
            }
        }
    }
}