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

    public class UnitOfWorkCommitExecutor<TContext> : IUnitOfWorkCommitExecutor<TContext>
            where TContext : IApplicationContext
    {
        [NotNull]
        readonly IConcurrencyConflictResolver<TContext> _conflictResolver;

        [NotNull]
        readonly IDbContextUpdateService _updateService;

        [NotNull]
        readonly IDbContextDeleteService _deleteService;

        [NotNull]
        readonly IDatabaseCommitManager _commitManager;

        public UnitOfWorkCommitExecutor([NotNull] IConcurrencyConflictResolver<TContext> conflictResolver,
                                        [NotNull] IDbContextUpdateService updateService,
                                        [NotNull] IDbContextDeleteService deleteService,
                                        [NotNull] IDatabaseCommitManager commitManager)
        {
            _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _commitManager = commitManager ?? throw new ArgumentNullException(nameof(commitManager));
        }

        public Task<UnitOfWorkCommitResult> ExecuteCommitAsync(IUnitOfWork unitOfWork)
        {
            return ExecuteCommitCoreAsync(unitOfWork, async db => await db.SaveChangesAsync(false).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public UnitOfWorkCommitResult ExecuteCommit(IUnitOfWork unitOfWork)
        {
            return ExecuteCommitCoreAsync(unitOfWork,
                                          db =>
                                          {
                                              db.SaveChanges(false);
                                              return Task.CompletedTask;
                                          }).Result;
        }

        async Task<UnitOfWorkCommitResult> ExecuteCommitCoreAsync(IUnitOfWork unitOfWork, Func<DbContext, Task> callback)
        {
            var _dbContext = unitOfWork.Context as DbContext;

            try
            {
                _dbContext.ChangeTracker.DetectChanges();

                if (!_dbContext.ChangeTracker.HasChanges())
                    return new UnitOfWorkCommitResult();

                var conflictResult = await _conflictResolver.ResolveAsync(unitOfWork.Context).ConfigureAwait(false);

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

                var changedAt = unitOfWork.UseTimeSourceFromEntities;

                _updateService.Apply(unitOfWork, changedAt);
                _deleteService.Apply(unitOfWork, changedAt);

                // save the database without appling changes
                await callback(_dbContext);

                // raise all changes
                _commitManager.RaiseCommited(typeof(TContext), GetEntries(_dbContext));

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