﻿// -----------------------------------------------------------------------
//  <copyright file="RepositoryCacheProxy.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Interfaces.Entities;
    using Interfaces.Specifications;
    using Interfaces.Stores;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Options;
    using Specifications;

    public class StoreCacheProxy<TEntity> : IStoreCached<TEntity>
            where TEntity : IEntity
    {
        [NotNull]
        readonly ILogger<StoreCacheProxy<TEntity>> _logger;

        [NotNull]
        readonly IStoreTransient<TEntity> _transient;

        [NotNull]
        readonly IMemoryCache _cache;

        [NotNull]
        readonly string _cacheKey = typeof(TEntity).Name;

        [NotNull]
        readonly EntityCacheOptions _options;

        public StoreCacheProxy([NotNull] ILogger<StoreCacheProxy<TEntity>> logger,
                               [NotNull] IStoreTransient<TEntity> transient,
                                    [NotNull] IMemoryCache cache,
                                    IOptions<StoreCacheOptions> options,
                                    IOptionsMonitor<EntityCacheOptions> singleOptions)
        {
            _logger = logger;
            _transient = transient;
            _cache     = cache;

            _options = singleOptions?.Get(typeof(TEntity).Name) ?? GetOptions(options?.Value ?? new StoreCacheOptions());
        }

        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default) => _transient.GetByIdAsync(id: id, cancellationToken: cancellationToken);

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync().ConfigureAwait(false);

            return set.Count;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult> DeleteAsync(TEntity entity)
        {
            var result = await _transient.DeleteAsync(entity: entity).ConfigureAwait(false);

            if (result.IsSuccessful)
            {
                await ReloadAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult> DeleteManyAsync(params TEntity[] entities)
        {
            var result = await _transient.DeleteManyAsync(entities: entities).ConfigureAwait(false);

            if (result.IsSuccessful)
            {
                await ReloadAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<TEntity>> InsertAsync(TEntity entity)
        {
            var result = await _transient.InsertAsync(entity: entity).ConfigureAwait(false);

            if (result.IsSuccessful)
            {
                await ReloadAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> InsertManyAsync(params TEntity[] entities)
        {
            var result = await _transient.InsertManyAsync(entities: entities).ConfigureAwait(false);

            if (result.IsSuccessful)
            {
                await ReloadAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<TEntity>> UpdateAsync(TEntity entity)
        {
            var result = await _transient.UpdateAsync(entity: entity).ConfigureAwait(false);

            if (result.IsSuccessful)
            {
                await ReloadAsync().ConfigureAwait(false);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> UpdateManyAsync(params TEntity[] entities)
        {
           var result = await _transient.UpdateManyAsync(entities: entities).ConfigureAwait(false);

           if (result.IsSuccessful)
           {
               await ReloadAsync().ConfigureAwait(false);
           }

           return result;
        }

        /// <inheritdoc />
        public async Task ReloadAsync()
        {
            _logger.LogDebug("Store cache ({Name}): reload", _cacheKey);

            var value = await _transient.GetAllAsync().ConfigureAwait(false);

            _cache.Set(key: _cacheKey, value: value, GetCacheOptions());
        }

        [NotNull]
        EntityCacheOptions GetOptions([NotNull] StoreCacheOptions options)
        {
            if (options.Entities.TryGetValue(key: typeof(TEntity).Name, out var entityCacheOptions))
                return entityCacheOptions;

            if (options.Entities.TryGetValue(typeof(TEntity).Name.Replace(oldValue: "Entity", newValue: ""), value: out entityCacheOptions))
                return entityCacheOptions;

            return options.Default;
        }

        MemoryCacheEntryOptions GetCacheOptions()
        {
            var cacheCancel = new CancellationTokenSource();

            var opt = new MemoryCacheEntryOptions();

            if (_options.AbsoluteExpirationRelativeToNow.HasValue)
                opt.SetAbsoluteExpiration(_options.AbsoluteExpirationRelativeToNow.Value);

            if (_options.SlidingExpiration.HasValue)
            {
                //opt.SetSlidingExpiration(_options.SlidingExpiration.Value);
                cacheCancel.CancelAfter(_options.SlidingExpiration.Value);
            }

            opt.AddExpirationToken(new CancellationChangeToken(cacheCancel.Token))
               .RegisterPostEvictionCallback(CacheEvictionCallback);

            return opt;
        }

        void CacheEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            _logger.LogDebug("Store cache ({Name}): eviction for {Key} because of {Reason}", _cacheKey, key, reason);
        }

        // TODO consider partial caching
        async Task<IReadOnlyList<TEntity>> ReloadAndGetItemsAsync()
        {
            var value = (await _transient.GetAllAsync().ConfigureAwait(false)).ToList().AsReadOnly();

            _cache.Set(key: _cacheKey, value: value, GetCacheOptions());

            return value;
        }

        public async Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector,
                                                                                    TSpecification specification,
                                                                                    CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync().ConfigureAwait(false);

            if (specification != null)
                set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return set.Select(entitySelector.Compile()).SingleOrDefault();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                                 TSpecification specification,
                                                                                                 CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync().ConfigureAwait(false);

            set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return set.Select(selector.Compile()).ToList().AsReadOnly();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                                  TSpecification specification,
                                                                                                  CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync().ConfigureAwait(false);

            set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return set.Select(selector.Compile()).ToList().AsReadOnly();
        }

        /// <inheritdoc />
        public async Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                                TSpecification specification,
                                                                                                CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var set = _cache.Get<IReadOnlyList<TEntity>>(key: _cacheKey) ?? await ReloadAndGetItemsAsync().ConfigureAwait(false);

            set = SpecificationHelper.Apply(collection: set, specification: specification).ToList();

            return PaginationHelper.Create(selector.Compile(), queryIteration: set, specification: specification);
        }
    }
}