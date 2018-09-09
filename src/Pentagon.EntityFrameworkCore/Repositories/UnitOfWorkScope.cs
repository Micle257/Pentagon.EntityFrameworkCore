// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkScope.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Linq;
    using Abstractions;
    using JetBrains.Annotations;

    public class UnitOfWorkScope<TContext> : IUnitOfWorkScope<TContext>
            where TContext : IApplicationContext
    {
        /// <summary> The unit of work factory. </summary>
        [NotNull]
        readonly IUnitOfWorkFactory<TContext> _unitOfWorkFactory;

        [NotNull]
        readonly IUnitOfWorkCommitExecutor<TContext> _commitExecutor;

        [NotNull]
        readonly IDataUserProvider _userProvider;

        /// <summary> The scoped unit of work. </summary>
        IUnitOfWork<TContext> _scopedUnitOfWork;

        public UnitOfWorkScope([NotNull] IUnitOfWorkFactory<TContext> unitOfWorkFactory,
                               [NotNull] IUnitOfWorkCommitExecutor<TContext> commitExecutor,
                               [NotNull] IDataUserProvider userProvider)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _commitExecutor = commitExecutor ?? throw new ArgumentNullException(nameof(commitExecutor));
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
        }

        public object UserId { get; set; }

        /// <inheritdoc />
        IUnitOfWork IUnitOfWorkScope.Get() => Get();

        /// <inheritdoc />
        public IUnitOfWork<TContext> Get()
        {
            if (_scopedUnitOfWork != null)
                throw new InvalidOperationException(message: "The unit of work is created for scope, disposed it first.");

            _scopedUnitOfWork = _unitOfWorkFactory.Create();
            _userProvider.UserId = UserId;

            return _scopedUnitOfWork;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // if the scope is open
            if (_scopedUnitOfWork != null)
            {
                try
                {
                    _commitExecutor.ExecuteCommit(_scopedUnitOfWork);
                }
                finally
                {
                    _scopedUnitOfWork?.Dispose();
                    _scopedUnitOfWork = null;
                }
            }
        }
    }
}