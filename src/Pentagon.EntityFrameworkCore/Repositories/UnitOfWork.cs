// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkCommitExecutor.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;

    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
            where TContext : IApplicationContext
    {
        [NotNull]
        readonly IConcurrencyConflictResolver<TContext> _conflictResolver;

        [NotNull]
        readonly IDbContextUpdateService _updateService;

        [NotNull]
        readonly IDbContextDeleteService _deleteService;
        
        public UnitOfWork([NotNull] IConcurrencyConflictResolver<TContext> conflictResolver,
                                        [NotNull] IDbContextUpdateService updateService,
                                        [NotNull] IDbContextDeleteService deleteService)
        {
            _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
        }

        public Task<UnitOfWorkCommitResult> ExecuteCommitAsync(IApplicationContext appContext)
        {
            return ExecuteCommitCoreAsync(appContext, async db => await db.SaveChangesAsync(false).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public UnitOfWorkCommitResult ExecuteCommit(IApplicationContext appContext)
        {
            return ExecuteCommitCoreAsync(appContext,
                                          db =>
                                          {
                                              db.SaveChanges(false);
                                              return Task.CompletedTask;
                                          }).Result;
        }

        async Task<UnitOfWorkCommitResult> ExecuteCommitCoreAsync(IApplicationContext appContext, Func<DbContext, Task> callback)
        {
            var _dbContext = appContext as DbContext;

            try
            {
                _dbContext.ChangeTracker.DetectChanges();

                if (!_dbContext.ChangeTracker.HasChanges())
                    return new UnitOfWorkCommitResult();

                var conflictResult = await _conflictResolver.ResolveAsync(appContext).ConfigureAwait(false);

                if (conflictResult.HasConflicts)
                {
                    return new UnitOfWorkCommitResult
                           {
                                   Conflicts = conflictResult.ConflictedEntities,
                                   Exception = new UnitOfWorkConcurrencyConflictException
                                               {
                                                       Conflicts = conflictResult.ConflictedEntities
                                               }
                           };
                }

                var changedAt = appContext.UseTimeSourceFromEntities;

                _updateService.Apply(appContext, changedAt);
                _deleteService.Apply(appContext, changedAt);

                // save the database without appling changes
                await callback(_dbContext);
                
                // accept changes
                _dbContext.ChangeTracker.AcceptAllChanges();

                return new UnitOfWorkCommitResult();
            }
            catch (Exception e)
            {
                return new UnitOfWorkCommitResult {Exception = e};
            }
        }

        IEnumerable<Entry> GetEntries(DbContext _dbContext)
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
                yield return new Entry(entry.Entity as IEntity, entry.State.ToEntityStateType());
        }
    }
}