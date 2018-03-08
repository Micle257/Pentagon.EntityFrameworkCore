namespace Pentagon.Data.EntityFramework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using JetBrains.Annotations;

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for paged operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetPageSpecification<TEntity> : IOrderSpecification<TEntity>, ICriteriaSpecification<TEntity>, IPaginationSpecification<TEntity>
        where TEntity : IEntity
    {
        /// <inheritdoc />
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            if (Criteria != null)
                query = query.Where(Criteria);

            if (Order != null)
            {
                if (IsDescending)
                    query = query.OrderByDescending(Order);
                else
                    query = query.OrderBy(Order);
            }

            return query;
        }

        /// <inheritdoc />
        public IList<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> Criteria { get; }

        /// <inheritdoc />
        public bool IsDescending { get; }

        /// <inheritdoc />
        public Expression<Func<TEntity, object>> Order { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPageSpecification{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="order">The order.</param>
        /// <param name="isDescending">If set to <c>true</c> is descending.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        public GetPageSpecification([NotNull] Expression<Func<TEntity, bool>> criteria, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending, int pageSize, int pageIndex)
        {
            Require.NotNull(() => criteria);
            Require.NotNull(() => order);
            Criteria = criteria;
            Order = order;
            IsDescending = isDescending;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        /// <inheritdoc />
        public int PageSize { get; set; }

        /// <inheritdoc />
        public int PageIndex { get; set; }

        /// <inheritdoc />
        public IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query)
        {
            if (PageSize > 0 && PageIndex >= 0)
                query = query.Skip((PageIndex - 1) * PageSize).Take(PageSize);

            return query;
        }
    }
}