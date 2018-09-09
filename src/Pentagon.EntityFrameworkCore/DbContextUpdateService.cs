﻿// -----------------------------------------------------------------------
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

    /// <summary> Represents an implementation of <see cref="IDbContextUpdateService" /> for entity framework core. </summary>
    public class DbContextUpdateService : IDbContextUpdateService
    {
        readonly IDataUserProvider _userProvider;

        public DbContextUpdateService(IDataUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        /// <inheritdoc />
        public void Apply(IUnitOfWork unitOfWork, bool useTimestampFromEntity)
        {
            var appContext = unitOfWork.Context;

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
                        identityName.CreatedUser = string.IsNullOrWhiteSpace(_userProvider.UserName) ? "dbo" : _userProvider.UserName;
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

                // generate new concurrency id both for add and update
                if (entry.Entity is IConcurrencyStampSupport concurrency)
                    concurrency.ConcurrencyStamp = Guid.NewGuid();
            }
        }
    }
}