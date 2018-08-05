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

        /// <summary>
        /// Initializes a new instance of the <see cref="GetManySpecification{TEntity}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="isDescending"> If set to <c> true </c> is descending. </param>
        public GetManySpecification([NotNull] Expression<Func<TEntity, object>> order, bool isDescending)
        {
            AddOrder(order ?? throw new ArgumentNullException(nameof(order)), isDescending);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetManySpecification{TEntity}"/> class.
        /// </summary>
        public GetManySpecification()
        {

        }

        /// <inheritdoc />
        [NotNull]
        public ICollection<Expression<Func<TEntity, bool>>> Filters { get; } = new List<Expression<Func<TEntity, bool>>>();

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
        [NotNull]
        public IList<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

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
                {
                    // apply condition to query
                    query = query.Where(filter);
                }
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
                    {
                        orderQuery = order.IsDescending ? orderQuery.ThenByDescending(order.Order) : orderQuery.ThenBy(order.Order);
                    }
                }

                query = orderQuery;
            }

            return query;
        }

        public IFilterSpecification<TEntity> AddTextFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value)
        {
            var expression = GetTextFilter(propertySelector, filter, value);

            Filters.Add(expression);

            return this;
        }

        public IFilterSpecification<TEntity> AddTextDoubleFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter firstFilter, string firstValue, FilterLogicOperation operation, TextFilter secondFilter, string secondValue)
        {
            var leftExpression = GetTextFilter(propertySelector, firstFilter, firstValue).Body;
            
            var rightExpression = GetTextFilter(propertySelector, secondFilter, secondValue).Body;

            ExpressionType expressionType;

            switch (operation)
            {
                case FilterLogicOperation.Or:
                    expressionType = ExpressionType.OrElse;
                    break;
                case FilterLogicOperation.And:
                    expressionType = ExpressionType.AndAlso;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            Expression expression = Expression.MakeBinary(expressionType, leftExpression, rightExpression);

            var parameter = Expression.Parameter(typeof(TEntity), "e");

            expression = new ParameterReplacer(parameter).Visit(expression);

            var lambda = (Expression<Func<TEntity, bool>>)Expression.Lambda(typeof(Func<TEntity, bool>), expression, parameter);

            Filters.Add(lambda);
            
            return this;
        }

        /// <inheritdoc />
        public object AddNumberFilter(Expression<Func<TEntity, decimal>> propertySelector, NumberFilter filter, decimal value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object AddNumberFilter(Expression<Func<TEntity, int>> propertySelector, NumberFilter filter, int value)
        {
            throw new NotImplementedException();
        }

        Expression<Func<TEntity, bool>> GetTextFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value)
        {
            var ex = GetTextFilterCallback(propertySelector.Body, filter, value);

            var parameter = Expression.Parameter(typeof(TEntity), "e");

            ex = new ParameterReplacer(parameter).Visit(ex);

            return (Expression<Func<TEntity, bool>>)Expression.Lambda(typeof(Func<TEntity, bool>), ex, parameter);
        }
        
        Expression GetTextFilterCallback(Expression callBody,TextFilter textFilter, string value)
        {
            switch (textFilter)
            {
                case TextFilter.Equal:
                    return GetNotInvertedBody(callBody,v => v.Equals(value));
                case TextFilter.NotEqual:
                    return GetInvertedBody(callBody,v => !v.Equals(value));
                case TextFilter.StartWith:
                    return GetNotInvertedBody(callBody,v => v.StartsWith(value));
                case TextFilter.EndWith:
                    return GetNotInvertedBody(callBody,v => v.EndsWith(value));
                case TextFilter.Contain:
                    return GetNotInvertedBody( callBody,v => v.Contains(value));
                case TextFilter.NotContain:
                    return GetInvertedBody( callBody,v => !v.Contains(value));
                default:
                    throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
            }
        }

        Expression GetNotInvertedBody(Expression callBody,Expression<Func<string, bool>> callback)
        {
            var body = ((MethodCallExpression)callback.Body);
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            var concatExpressionBody = Expression.Call(callBody, containsMethodInfo, containsArgument);

            return concatExpressionBody;
        }

        Expression GetInvertedBody(Expression callBody, Expression<Func<string, bool>> callback)
        {
            var body = (MethodCallExpression)((UnaryExpression)callback.Body).Operand;
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            var concatExpressionBody = Expression.Not(Expression.Call(callBody, containsMethodInfo, containsArgument));

            return concatExpressionBody;
        }
    }
}