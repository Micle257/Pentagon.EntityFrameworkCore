namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using Specifications;

    public interface IStore<TEntity> : IDeleteStore<TEntity>,
                                       IInsertStore<TEntity>,
                                       IUpdateStore<TEntity>,
                                       IPagedStore<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the selected entity. </summary>
        /// <param name="entitySelector"> The entity selector. </param>
        /// <returns> An awaitable <see cref="Task{TEntity}" />. </returns>
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector, CancellationToken cancellationToken = default);

        Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>;

        /// <summary> Gets the entity by id. </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns> An awaitable <see cref="Task{TEntity}" />. </returns>
        Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        /// <summary> Gets all entities in the set. </summary>
        /// <returns> A <see cref="Task" /> that represents an asynchronous operation, result is an iterator of the <see cref="TEntity" />. </returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>;

        /// <summary> Gets all selected entities from the set. </summary>
        /// <param name="entitiesSelector"> The entities selector. </param>
        /// <param name="orderSelector"> The order selector. </param>
        /// <param name="isDescending"> If set to <c> true </c> entities will be ordered from highest to lowest. </param>
        /// <returns> A <see cref="Task" /> that represents an asynchronous operation, result is an iterator of the <see cref="TEntity" />. </returns>
        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector,
                                                Expression<Func<TEntity, object>> orderSelector,
                                                bool isDescending,
                                                CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>;

        /// <summary> Counts rows in repository. </summary>
        /// <returns> A <see cref="Task" /> that represents the asynchronous operation. Result of the operation is number of rows in repository. </returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<TSelectEntity> GetOneAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate, CancellationToken cancellationToken = default);

        Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>;

        Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, CancellationToken cancellationToken = default);

        Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>;

        Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                     Expression<Func<TEntity, bool>> entitiesSelector,
                                                                     Expression<Func<TEntity, object>> orderSelector,
                                                                     bool isDescending,
                                                                     CancellationToken cancellationToken = default);

        Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>;

        Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                   Expression<Func<TEntity, bool>> criteria,
                                                                   Expression<Func<TEntity, object>> order,
                                                                   bool isDescendingOrder,
                                                                   int pageSize,
                                                                   int pageIndex,
                                                                   CancellationToken cancellationToken = default);

        Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>;
    }
}