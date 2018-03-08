﻿namespace Pentagon.Data.EntityFramework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using JetBrains.Annotations;

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for get all pages operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetAllPagesSpecification<TEntity> : ICriteriaSpecification<TEntity>, IOrderSpecification<TEntity>, IAllPaginationSpecification<TEntity>
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
        public bool IsDescending { get; }

        /// <inheritdoc />
        public Expression<Func<TEntity, object>> Order { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPagesSpecification{TEntity}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="isDescending">If set to <c>true</c> is descending.</param>
        /// <param name="pageSize">Size of the page.</param>
        public GetAllPagesSpecification([NotNull] Expression<Func<TEntity, bool>> criteria, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending, int pageSize) 
        {
            Require.NotNull(() => criteria);
            Require.NotNull(() => order);
            Criteria = criteria;
            Order = order;
            IsDescending = isDescending;
            PageSize = pageSize;
        }

        /// <inheritdoc />
        public int PageSize { get; }
        
        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> Criteria { get; }
    }
}