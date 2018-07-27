// -----------------------------------------------------------------------
//  <copyright file="GetManySpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using JetBrains.Annotations;

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for get many operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetManySpecification<TEntity> : IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Initializes a new instance of the <see cref="GetManySpecification{TEntity}" /> class. </summary>
        /// <param name="filter"> The filter. </param>
        /// <param name="order"> The order. </param>
        /// <param name="isDescending"> If set to <c> true </c> is descending. </param>
        public GetManySpecification([NotNull] Expression<Func<TEntity, bool>> filter, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            Order = order ?? throw new ArgumentNullException(nameof(order));
            IsDescending = isDescending;
        }

        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> Filter { get; }

        /// <inheritdoc />
        public bool IsDescending { get; }

        /// <inheritdoc />
        public Expression<Func<TEntity, object>> Order { get; }

        /// <inheritdoc />
        public IList<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

        /// <inheritdoc />
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            if (Filter != null)
                query = query.Where(Filter);

            if (Order != null)
            {
                if (IsDescending)
                    query = query.OrderByDescending(Order);
                else
                    query = query.OrderBy(Order);
            }

            return query;
        }
    }
}