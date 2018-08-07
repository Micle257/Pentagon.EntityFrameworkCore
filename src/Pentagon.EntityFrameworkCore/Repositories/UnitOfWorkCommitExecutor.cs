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
        readonly IDbContextIdentityService _identityService;

        [NotNull]
        readonly IDatabaseCommitManager _commitManager;

        public UnitOfWorkCommitExecutor([NotNull] IConcurrencyConflictResolver<TContext> conflictResolver,
                                        [NotNull] IDbContextUpdateService updateService,
                                        [NotNull] IDbContextDeleteService deleteService,
                                        [NotNull] IDbContextIdentityService identityService,
                                        [NotNull] IDatabaseCommitManager commitManager)
        {
            _conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _commitManager = commitManager ?? throw new ArgumentNullException(nameof(commitManager));
        }

        public async Task<UnitOfWorkCommitResult> ExecuteCommitAsync(IUnitOfWork<TContext> unitOfWork)
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

                _updateService.Apply(unitOfWork.Context);
                _deleteService.Apply(unitOfWork.Context, unitOfWork.Context.HasHardDeleteBehavior);
                _identityService.Apply(unitOfWork.Context, unitOfWork.UserId);

                // save the database without appling changes
                await _dbContext.SaveChangesAsync(false).ConfigureAwait(false);

                // raise all changes
                _commitManager.RaiseCommited(typeof(TContext), GetEntries(_dbContext));

                // accept changes
                _dbContext.ChangeTracker.AcceptAllChanges();

                return new UnitOfWorkCommitResult();
            }
            catch (Exception e)
            {
                return new UnitOfWorkCommitResult { Exception =  e};
            }
        }

        /// <inheritdoc />
        public UnitOfWorkCommitResult ExecuteCommit(IUnitOfWork<TContext> unitOfWork)
        {
            var _dbContext = unitOfWork.Context as DbContext;

            try
            {
                _dbContext.ChangeTracker.DetectChanges();

                if (!_dbContext.ChangeTracker.HasChanges())
                    return new UnitOfWorkCommitResult();

                var conflictResult = _conflictResolver.ResolveAsync(unitOfWork.Context).Result;

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

                _updateService.Apply(unitOfWork.Context);
                _deleteService.Apply(unitOfWork.Context, unitOfWork.Context.HasHardDeleteBehavior);
                _identityService.Apply(unitOfWork.Context, unitOfWork.UserId);

                // save the database without appling changes
                _dbContext.SaveChanges(false);

                // raise all changes
                _commitManager.RaiseCommited(typeof(TContext), GetEntries(_dbContext));

                // accept changes
                _dbContext.ChangeTracker.AcceptAllChanges();

                return new UnitOfWorkCommitResult();
            }
            catch (Exception e)
            {
                return new UnitOfWorkCommitResult { Exception =  e};
            }
        }

        IEnumerable<Entry> GetEntries(DbContext _dbContext)
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
                yield return new Entry(entry.Entity as IEntity, entry.State.ToEntityStateType());
        }
    }
}