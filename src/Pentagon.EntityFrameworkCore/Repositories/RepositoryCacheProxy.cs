﻿// -----------------------------------------------------------------------
//  <copyright file="RepositoryCacheProxy.cs">
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
    using Interfaces.Entities;
    using Interfaces.Repositories;
    using Interfaces.Specifications;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Options;
    using Specifications;

    public class RepositoryCacheProxy<TEntity> : IRepositoryCached<TEntity>
            where TEntity : IEntity
    {
        [NotNull]
        readonly IRepositoryTransient<TEntity> _transient;

        [NotNull]
        readonly IMemoryCache _cache;

        [NotNull]
        readonly string _cacheKey = typeof(TEntity).Name;

        [NotNull]
        readonly EntityCacheOptions _options;

        public RepositoryCacheProxy([NotNull] IRepositoryTransient<TEntity> transient,
                                    [NotNull] IMemoryCache cache,
                                    IOptions<RepositoryCacheOptions> options)
        {
            _transient = transient;
            _cache     = cache;

            _options = GetOptions(options?.Value ?? new RepositoryCacheOptions());
        }

        /// <inheritdoc />
        public Type ElementType => _transient.AsQueryable().ElementType;

        /// <inheritdoc />
        public Expression Expression => _transient.AsQueryable().Expression;

        /// <inheritdoc />
        public IQueryProvider Provider => _transient.AsQueryable().Provider;

        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default) => _transient.GetByIdAsync(id: id, cancellationToken: cancellationToken);

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync();

            return set.Count;
        }

        /// <inheritdoc />
        public virtual void Insert(TEntity entity)
        {
            _transient.Insert(entity: entity);
        }

        /// <inheritdoc />
        public virtual void InsertMany([NotNull] params TEntity[] entities)
        {
            _transient.InsertMany(entities: entities);
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            _transient.Update(entity: entity);
        }

        /// <inheritdoc />
        public void UpdateMany(params TEntity[] entities)
        {
            _transient.UpdateMany(entities: entities);
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity entity)
        {
            _transient.Delete(entity: entity);
        }

        /// <inheritdoc />
        public void DeleteMany(params TEntity[] entities)
        {
            _transient.DeleteMany(entities: entities);
        }

        /// <inheritdoc />
        public void Truncate()
        {
            _transient.Truncate();
        }

        /// <inheritdoc />
        public async Task ReloadAsync()
        {
            var value = await _transient.GetAllAsync();

            _cache.Set(key: _cacheKey, value: value, GetCacheOptions());
        }

        /// <inheritdoc />
        public IEnumerator<TEntity> GetEnumerator() => _transient.AsQueryable().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [NotNull]
        EntityCacheOptions GetOptions([NotNull] RepositoryCacheOptions options)
        {
            if (options.Entities.TryGetValue(key: typeof(TEntity).Name, out var entityCacheOptions))
                return entityCacheOptions;

            if (options.Entities.TryGetValue(typeof(TEntity).Name.Replace(oldValue: "Entity", newValue: ""), value: out entityCacheOptions))
                return entityCacheOptions;

            return options.Default;
        }

        MemoryCacheEntryOptions GetCacheOptions()
        {
            var opt = new MemoryCacheEntryOptions();

            opt.AbsoluteExpiration              = _options.AbsoluteExpiration;
            opt.AbsoluteExpirationRelativeToNow = _options.AbsoluteExpirationRelativeToNow;
            opt.SlidingExpiration               = _options.SlidingExpiration;

            return opt;
        }

        async Task<IReadOnlyList<TEntity>> ReloadAndGetItemsAsync()
        {
            var value = (await _transient.GetAllAsync().ConfigureAwait(false)).ToList().AsReadOnly();

            _cache.Set(key: _cacheKey, value: value, GetCacheOptions());

            return value;
        }

        #region GetOne

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector, CancellationToken cancellationToken = default)
        {
            return GetOneAsync(e => e, entityPredicate: entitySelector, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<TSelectEntity> GetOneAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate, CancellationToken cancellationToken = default)
        {
            var spec = new GetOneSpecification<TEntity>(filter: entityPredicate);

            return GetOneAsync(entitySelector: selector, specification: spec, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            return GetOneAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        public async Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector,
                                                                                    TSpecification specification,
                                                                                    CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync();

            if (specification != null)
                set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return set.Select(entitySelector.Compile()).SingleOrDefault();
        }

        #endregion

        #region GetAll

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return GetAllAsync(e => e, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, CancellationToken cancellationToken = default)
        {
            var spec = new GetAllSpecification<TEntity>();

            return GetAllAsync(selector: selector, specification: spec, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            return GetAllAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                                 TSpecification specification,
                                                                                                 CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync();

            if (specification != null)
                set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return set.Select(selector.Compile()).ToList();
        }

        #endregion

        #region GetMany

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector,
                                                       Expression<Func<TEntity, object>> orderSelector,
                                                       bool isDescending,
                                                       CancellationToken cancellationToken = default)
        {
            return GetManyAsync(e => e, entitiesSelector: entitiesSelector, orderSelector: orderSelector, isDescending: isDescending, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                            Expression<Func<TEntity, bool>> entitiesSelector,
                                                                            Expression<Func<TEntity, object>> orderSelector,
                                                                            bool isDescending,
                                                                            CancellationToken cancellationToken = default)
        {
            var spec = new GetManySpecification<TEntity>(filter: entitiesSelector, order: orderSelector, isDescending: isDescending);

            return GetManyAsync(selector: selector, specification: spec, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            return GetManyAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                                  TSpecification specification,
                                                                                                  CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync();

            if (specification != null)
                set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return set.Select(selector.Compile()).ToList();
        }

        #endregion

        #region GetPage

        public Task<PagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria,
                                                     Expression<Func<TEntity, object>> order,
                                                     bool isDescendingOrder,
                                                     int pageSize,
                                                     int pageIndex,
                                                     CancellationToken cancellationToken = default)
        {
            return GetPageAsync(e => e, criteria: criteria, order: order, isDescendingOrder: isDescendingOrder, pageSize: pageSize, pageIndex: pageIndex, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                          Expression<Func<TEntity, bool>> criteria,
                                                                          Expression<Func<TEntity, object>> order,
                                                                          bool isDescendingOrder,
                                                                          int pageSize,
                                                                          int pageIndex,
                                                                          CancellationToken cancellationToken = default)
        {
            var specification = new GetPageSpecification<TEntity>(filter: criteria, order: order, isDescending: isDescendingOrder, pageSize: pageSize, pageNumber: pageIndex);

            return GetPageAsync(selector: selector, specification: specification, cancellationToken: cancellationToken);
        }

        public Task<PagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            return GetPageAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                                TSpecification specification,
                                                                                                CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync();

            if (specification != null)
                set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return PaginationHelper.Create(selector.Compile(), queryIteration: set, specification: specification);
        }

        #endregion
    }
}