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

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for get many/all operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetManySpecification<TEntity> : IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
            where TEntity : IEntity
    {
        [NotNull]
        readonly List<SpecificationOrder<TEntity>> _orders = new List<SpecificationOrder<TEntity>>();

        /// <summary> Initializes a new instance of the <see cref="GetManySpecification{TEntity}" /> class. </summary>
        /// <param name="filter"> The filter. </param>
        /// <param name="order"> The order. </param>
        /// <param name="isDescending"> If set to <c> true </c> is descending. </param>
        public GetManySpecification([NotNull] Expression<Func<TEntity, bool>> filter, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending)
        {
            Filters.Add(filter ?? throw new ArgumentNullException(nameof(filter)));
            AddOrder(order ?? throw new ArgumentNullException(nameof(order)), isDescending);
        }

        /// <summary> Initializes a new instance of the <see cref="GetManySpecification{TEntity}" /> class. </summary>
        /// <param name="order"> The order. </param>
        /// <param name="isDescending"> If set to <c> true </c> is descending. </param>
        public GetManySpecification([NotNull] Expression<Func<TEntity, object>> order, bool isDescending)
        {
            AddOrder(order ?? throw new ArgumentNullException(nameof(order)), isDescending);
        }

        /// <summary> Initializes a new instance of the <see cref="GetManySpecification{TEntity}" /> class. </summary>
        public GetManySpecification() { }

        /// <inheritdoc />
        [NotNull]
        public List<Expression<Func<TEntity, bool>>> Filters { get; } = new List<Expression<Func<TEntity, bool>>>();

        /// <inheritdoc />
        [NotNull]
        public IReadOnlyList<SpecificationOrder<TEntity>> Orders => _orders;
        
        /// <inheritdoc />
        public IOrderSpecification<TEntity> AddOrder(Expression<Func<TEntity, object>> order, bool isDescending)
        {
            _orders.Add(new SpecificationOrder<TEntity>
                        {
                                Order = order,
                                IsDescending = isDescending
                        });

            return this;
        }

        /// <inheritdoc />
        public IQueryable<TEntity> Apply([NotNull] IQueryable<TEntity> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            // if we have filters
            if (Filters.Count != 0)
            {
                // for each of filter
                foreach (var filter in Filters)
                        // apply condition to query
                    query = query.Where(filter);
            }

            if (Orders.Count != 0)
            {
                var orderQuery = default(IOrderedQueryable<TEntity>);

                for (var i = 0; i < Orders.Count; i++)
                {
                    var order = Orders[i];

                    if (i == 0)
                        orderQuery = order.IsDescending ? query.OrderByDescending(order.Order) : query.OrderBy(order.Order);
                    else
                        orderQuery = order.IsDescending ? orderQuery.ThenByDescending(order.Order) : orderQuery.ThenBy(order.Order);
                }

                query = orderQuery;
            }

            return query;
        }
    }
}