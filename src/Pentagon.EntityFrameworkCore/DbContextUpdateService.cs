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
        /// <inheritdoc />
        public void Apply<TContext>(IUnitOfWork<TContext> unitOfWork, DateTimeOffset changedAt)
                where TContext : IApplicationContext
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
                    if (entry.Entity is ICreatedTimeStampSupport entityTimed)
                        entityTimed.CreatedAt = changedAt;

                    if (entry.Entity is ICreateStampSupport createStamp)
                        createStamp.CreateGuid = Guid.NewGuid();

                    if (entry.Entity is ITimeStampIdentitySupport identity)
                        identity.CreatedBy = unitOfWork.UserId;
                }

                // set last updated at when the entity has modified
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IUpdatedTimeStampSupport entityTimed2)
                        entityTimed2.LastUpdatedAt = changedAt;

                    if (entry.Entity is ITimeStampIdentitySupport identity)
                        identity.UpdatedBy = unitOfWork.UserId;
                }

                // generate new concurrency id both for add and update
                if (entry.Entity is IConcurrencyStampSupport concurrency)
                    concurrency.ConcurrencyStamp = Guid.NewGuid();
            }
        }
    }
}