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
        /// <summary> The repository factory. </summary>
        [NotNull]
        readonly IRepositoryFactory _repositoryFactory;

        /// <summary> The update service. </summary>
        [NotNull]
        readonly IDbContextUpdateService _updateService;

        /// <summary> The update service. </summary>
        [NotNull]
        readonly IDbContextDeleteService _deleteService;

        /// <summary> The identity service. </summary>
        [NotNull]
        readonly IDbContextIdentityService _identityService;

        [NotNull]
        readonly IDatabaseCommitManager _commitManager;

        /// <summary> The context factory. </summary>
        [NotNull]
        readonly IContextFactory<TContext> _contextFactory;

        /// <summary> Initializes a new instance of the <see cref="UnitOfWorkFactory{TContext}" /> class. </summary>
        /// <param name="contextFactory"> The context. </param>
        /// <param name="repositoryFactory"> The repository factory. </param>
        /// <param name="updateService"> The update service. </param>
        /// <param name="deleteService"> The delete service. </param>
        /// <param name="identityService"> The identity service. </param>
        /// <param name="commitManager"> The commit manager. </param>
        public UnitOfWorkFactory([NotNull] IContextFactory<TContext> contextFactory,
                                 [NotNull] IRepositoryFactory repositoryFactory,
                                 [NotNull] IDbContextUpdateService updateService,
                                 [NotNull] IDbContextDeleteService deleteService,
                                 [NotNull] IDbContextIdentityService identityService,
                                 [NotNull] IDatabaseCommitManager commitManager)
        {
            Require.NotNull(() => commitManager);
            Require.NotNull(() => identityService);
            Require.NotNull(() => deleteService);
            Require.NotNull(() => updateService);
            Require.NotNull(() => repositoryFactory);
            _repositoryFactory = repositoryFactory;
            _updateService = updateService;
            _deleteService = deleteService;
            _identityService = identityService;
            _commitManager = commitManager;
            _contextFactory = contextFactory;
        }

        /// <inheritdoc />
        public IUnitOfWork<TContext> Create() => new UnitOfWork<TContext>(_contextFactory.CreateContext(), _repositoryFactory, _updateService, _deleteService, _identityService, _commitManager);
    }
}