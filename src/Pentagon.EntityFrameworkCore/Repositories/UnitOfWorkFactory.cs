// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using Abstractions;
    using Abstractions.Repositories;
    using JetBrains.Annotations;
    
    /// <summary> Represents a default implementation for an <see cref="IUnitOfWorkFactory{TContext}" />. </summary>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    public class UnitOfWorkFactory<TContext> : IUnitOfWorkFactory<TContext>
            where TContext : class, IApplicationContext
    {
        [NotNull]
        readonly IDatabaseCommitManager _commitManager;

        /// <summary> The context factory. </summary>
        [NotNull]
        readonly IContextFactory<TContext> _contextFactory;

        /// <summary> Initializes a new instance of the <see cref="UnitOfWorkFactory{TContext}" /> class. </summary>
        /// <param name="contextFactory"> The context. </param>
        /// <param name="repositoryFactory"> The repository factory. </param>
        /// <param name="commitManager"> The commit manager. </param>
        public UnitOfWorkFactory([NotNull] IContextFactory<TContext> contextFactory,
                                 [NotNull] IDatabaseCommitManager commitManager)
        {
            _commitManager = commitManager;
            _contextFactory = contextFactory;
        }

        /// <inheritdoc />
        IUnitOfWork IUnitOfWorkFactory.Create() => Create();

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create() => new UnitOfWork<TContext>(_contextFactory.CreateContext(),
                                                                          _commitManager);
    }
}