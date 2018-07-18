// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkScope.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using Abstractions;
    using JetBrains.Annotations;

    public class UnitOfWorkScope<TContext> : IUnitOfWorkScope<TContext>
            where TContext : IApplicationContext
    {
        /// <summary> The unit of work factory. </summary>
        readonly IUnitOfWorkFactory<TContext> _unitOfWorkFactory;

        readonly IUnitOfWorkCommitExecutor<TContext> _commitExecutor;

        /// <summary> The scoped unit of work. </summary>
        IUnitOfWork<TContext> _scopedUnitOfWork;

        public UnitOfWorkScope([NotNull] IUnitOfWorkFactory<TContext> unitOfWorkFactory,
                               [NotNull] IUnitOfWorkCommitExecutor<TContext> commitExecutor)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _commitExecutor = commitExecutor ?? throw new ArgumentNullException(nameof(commitExecutor));
        }

        public object UserId { get; set; }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Get()
        {
            if (_scopedUnitOfWork != null)
                throw new InvalidOperationException(message: "The unit of work is created for scope, disposed it first.");

            _scopedUnitOfWork = _unitOfWorkFactory.Create();
            _scopedUnitOfWork.UserId = UserId;

            return _scopedUnitOfWork;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // if the scope is open
            if (_scopedUnitOfWork != null)
            {
                _commitExecutor.ExecuteCommitAsync(_scopedUnitOfWork).Wait(); // TODO handle error
                _scopedUnitOfWork?.Dispose();
                _scopedUnitOfWork = null;
            }
        }
    }
}