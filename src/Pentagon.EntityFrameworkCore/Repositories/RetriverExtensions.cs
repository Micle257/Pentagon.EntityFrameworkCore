namespace Pentagon.EntityFrameworkCore.Repositories {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Interfaces.Entities;
    using Interfaces.Specifications;
    using Interfaces.Stores;
    using Specifications;

    public static class RetriverExtensions
    {
        #region GetOne

        /// <inheritdoc />
        public static Task<TEntity> GetOneAsync<TEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, bool>> entitySelector, CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            return store.GetOneAsync(e => e, entityPredicate: entitySelector, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<TSelectEntity> GetOneAsync<TEntity, TSelectEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate, CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            var spec = new GetOneSpecification<TEntity>(filter: entityPredicate);

            return store.GetOneAsync(entitySelector: selector, specification: spec, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<TEntity> GetOneAsync<TEntity , TSpecification>(this IRetriever<TEntity> store, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
                where TEntity : IEntity
        {
            return store.GetOneAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        #endregion

        #region GetAll

        /// <inheritdoc />
        public static Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this IRetriever<TEntity> store, CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            return store.GetAllAsync(e => e, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<IEnumerable<TSelectEntity>> GetAllAsync<TEntity, TSelectEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, TSelectEntity>> selector, CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            var spec = new GetAllSpecification<TEntity>();

            return store.GetAllAsync(selector: selector, specification: spec, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<IEnumerable<TEntity>> GetAllAsync<TEntity, TSpecification>(this IRetriever<TEntity> store, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
                where TEntity : IEntity
        {
            return store.GetAllAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        #endregion

        #region GetMany

        /// <inheritdoc />
        public static Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, bool>> entitiesSelector,
                                                                       Expression<Func<TEntity, object>> orderSelector,
                                                                       bool isDescending,
                                                                       CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            return store.GetManyAsync(e => e, entitiesSelector: entitiesSelector, orderSelector: orderSelector, isDescending: isDescending, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<IEnumerable<TSelectEntity>> GetManyAsync<TEntity, TSelectEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                            Expression<Func<TEntity, bool>> entitiesSelector,
                                                                                            Expression<Func<TEntity, object>> orderSelector,
                                                                                            bool isDescending,
                                                                                            CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            var spec = new GetManySpecification<TEntity>(filter: entitiesSelector, order: orderSelector, isDescending: isDescending);

            return store.GetManyAsync(selector: selector, specification: spec, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TSpecification>(this IRetriever<TEntity> store, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
                where TEntity : IEntity
        {
            return store.GetManyAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        #endregion

        #region GetPage

        public static Task<PagedList<TEntity>> GetPageAsync<TEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, bool>> criteria,
                                                                     Expression<Func<TEntity, object>> order,
                                                                     bool isDescendingOrder,
                                                                     int pageSize,
                                                                     int pageIndex,
                                                                     CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            return store.GetPageAsync(e => e, criteria: criteria, order: order, isDescendingOrder: isDescendingOrder, pageSize: pageSize, pageIndex: pageIndex, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public static Task<PagedList<TSelectEntity>> GetPageAsync<TEntity, TSelectEntity>(this IRetriever<TEntity> store, Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                          Expression<Func<TEntity, bool>> criteria,
                                                                                          Expression<Func<TEntity, object>> order,
                                                                                          bool isDescendingOrder,
                                                                                          int pageSize,
                                                                                          int pageIndex,
                                                                                          CancellationToken cancellationToken = default)
                where TEntity : IEntity
        {
            var specification = new GetPageSpecification<TEntity>(filter: criteria, order: order, isDescending: isDescendingOrder, pageSize: pageSize, pageNumber: pageIndex);

            return store.GetPageAsync(selector: selector, specification: specification, cancellationToken: cancellationToken);
        }

        public static Task<PagedList<TEntity>> GetPageAsync<TEntity, TSpecification>(this IRetriever<TEntity> store, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
                where TEntity : IEntity
        {
            return store.GetPageAsync(e => e, specification: specification, cancellationToken: cancellationToken);
        }

        #endregion

    }
}