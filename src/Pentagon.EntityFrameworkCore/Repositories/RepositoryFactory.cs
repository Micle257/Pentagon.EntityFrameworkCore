// -----------------------------------------------------------------------
//  <copyright file="RepositoryFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary> Represents a repository factory for <see cref="IApplicationContext" />. </summary>
    public class RepositoryFactory : IRepositoryFactory
    {
        /// <summary> The logger factory. </summary>
        [NotNull]
        readonly ILoggerFactory _loggerFactory;

        /// <summary> The pagination service. </summary>
        [NotNull]
        readonly IPaginationService _paginationService;

        /// <summary> The repository mappings. </summary>
        [NotNull]
        readonly IDictionary<(Type entity, object context), object> _repositories = new Dictionary<(Type entity, object context), object>();

        /// <summary> Initializes a new instance of the <see cref="RepositoryFactory" /> class. </summary>
        /// <param name="logger"> The logger. </param>
        /// <param name="paginationService"> The pagination service. </param>
        public RepositoryFactory([NotNull] ILoggerFactory logger, [NotNull] IPaginationService paginationService)
        {
            _loggerFactory = logger ?? throw new ArgumentNullException(nameof(logger));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
        }

        /// <inheritdoc />
        public IRepository<TEntity> GetRepository<TEntity>(IApplicationContext context)
            where TEntity : class, IEntity, new()
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Require.IsType(() => context, out DbContext dbContext);

            return new Repository<TEntity>(_loggerFactory.CreateLogger<Repository<TEntity>>(), _paginationService, dbContext);

            var type = typeof(TEntity);

            if (!_repositories.ContainsKey((type,dbContext)))
                _repositories[(type, dbContext)] = new Repository<TEntity>(_loggerFactory.CreateLogger<Repository<TEntity>>(), _paginationService, dbContext);

            if (!(_repositories[(type, dbContext)] is IRepository<TEntity>))
                throw new ArgumentException(message: "The requested repository is not of return type.");

            return (IRepository<TEntity>) _repositories[(type, dbContext)];
        }
    }
}