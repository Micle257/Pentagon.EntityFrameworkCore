// -----------------------------------------------------------------------
//  <copyright file="Repository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Interfaces;
    using Interfaces.Entities;
    using Interfaces.Repositories;
    using Interfaces.Specifications;
    using Interfaces.Stores;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pentagon.Extensions.Logging;
    using Specifications;

    /// <summary> Represents a repository for the entity framework provider. It has similar behavior like <see cref="DbSet{TEntity}" />. Marks and gets data from database. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class StoreTransient<TEntity> : IStoreTransient<TEntity>
            where TEntity : class, IEntity, new()
    {
        [NotNull]
        readonly ILogger<StoreTransient<TEntity>> _logger;

        [NotNull]
        readonly IContextFactory _contextFactory;

        /// <summary> Initializes a new instance of the <see cref="Repository{TEntity}" /> class. </summary>
        /// <param name="context"> The database context. </param>
        public StoreTransient([NotNull] ILogger<StoreTransient<TEntity>> logger,
                              [NotNull] IContextFactory context)
        {
            _logger = logger;
            _contextFactory = context;
        }

        [NotNull]
        IApplicationContext CreateContext()
        {
            _logger.LogDebug("Creating application context.");

            return _contextFactory.CreateContext();
        }

        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            using var _ = _logger.LogMethod($"Id: {id}");

            var repository = _contextFactory.CreateContext().GetRepository<TEntity>();

            return repository.GetByIdAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            var repository = CreateContext().GetRepository<TEntity>();

            return repository.CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult> DeleteAsync(TEntity entity)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            using var context = CreateContext();

            var repository = context.GetRepository<TEntity>();

            repository.Delete(entity);

            var result = await context.ExecuteCommitAsync().ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult> DeleteManyAsync(params TEntity[] entities)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            using var context = CreateContext();

            var repository = context.GetRepository<TEntity>();

            repository.DeleteMany(entities);

            var result = await context.ExecuteCommitAsync().ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<TEntity>> InsertAsync(TEntity entity)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            using var context = CreateContext();

            var repository = context.GetRepository<TEntity>();

            repository.Insert(entity);

            var result = await context.ExecuteCommitAsync().ConfigureAwait(false);

            return new ContextCommitResult<TEntity>
            {
                Result = entity,
                Exception = result.Exception,
                Conflicts = result.Conflicts,
                Content = result.Content
            };
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> InsertManyAsync(params TEntity[] entities)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            using var context = CreateContext();

            var repository = context.GetRepository<TEntity>();

            repository.InsertMany(entities);

            var result = await context.ExecuteCommitAsync().ConfigureAwait(false);

            return new ContextCommitResult<IReadOnlyCollection<TEntity>>
            {
                Result = entities,
                Exception = result.Exception,
                Conflicts = result.Conflicts,
                Content = result.Content
            };
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<TEntity>> UpdateAsync(TEntity entity)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            using var context = CreateContext();

            var repository = context.GetRepository<TEntity>();

            repository.Update(entity);

            var result = await context.ExecuteCommitAsync().ConfigureAwait(false);

            return new ContextCommitResult<TEntity>
            {
                Result = entity,
                Exception = result.Exception,
                Conflicts = result.Conflicts,
                Content = result.Content
            };
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> UpdateManyAsync(params TEntity[] entities)
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            using var context = CreateContext();

            var repository = context.GetRepository<TEntity>();

            repository.UpdateMany(entities);

            var result = await context.ExecuteCommitAsync().ConfigureAwait(false);

            return new ContextCommitResult<IReadOnlyCollection<TEntity>>
            {
                Result = entities,
                Exception = result.Exception,
                Conflicts = result.Conflicts,
                Content = result.Content
            };
        }

        public Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            var repository = CreateContext().GetRepository<TEntity>();

            return repository.GetOneAsync(entitySelector, specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            var repository = CreateContext().GetRepository<TEntity>();

            return repository.GetAllAsync(selector, specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            var repository = CreateContext().GetRepository<TEntity>();

            return repository.GetManyAsync(selector, specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            using var _ = _logger.LogMethod();
            using var __ = _logger.InScope(("EntityType", typeof(TEntity).Name));

            var repository = CreateContext().GetRepository<TEntity>();

            return repository.GetPageAsync(selector, specification, cancellationToken);
        }
    }
}