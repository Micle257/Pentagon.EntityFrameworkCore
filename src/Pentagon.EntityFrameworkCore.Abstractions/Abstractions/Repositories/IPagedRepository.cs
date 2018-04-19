namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Entities;
    using Specifications;

    public interface IPagedRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Get all entities in pages.
        /// </summary>
        /// <param name="pageSize">Size of a page.</param>
        /// <param name="orderExpression">The order expression.</param>
        /// <returns>
        /// A <see cref="Task" /> that represents the asynchronous operation. The result of the operation is an iteration of the entities paged list.
        /// </returns>
        Task<IEnumerable<IPagedList<TEntity>>> GetAllPagesAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> orderExpression, bool isDescendingOrder, int pageSize);

        Task<IEnumerable<IPagedList<TEntity>>> GetAllPagesAsync<TSpecification>(TSpecification specification)
            where TSpecification : IAllPaginationSpecification<TEntity>, ICriteriaSpecification<TEntity>, IOrderSpecification<TEntity>;

        Task<IPagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification)
            where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, ICriteriaSpecification<TEntity>;

        Task<IPagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex);
    }
}