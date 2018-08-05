// -----------------------------------------------------------------------
//  <copyright file="FilterSpecificationExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public static class FilterSpecificationExtensions
    {
        public static IFilterSpecification<TEntity> AddTextFilter<TEntity>(this IFilterSpecification<TEntity> specification, Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value)
                where TEntity : IEntity
        {
            var expression = TextFilterExpressionHelper.GetFilter(propertySelector, filter, value);

            specification.Filters.Add(expression);

            return specification;
        }

        public static IFilterSpecification<TEntity> AddTextDoubleFilter<TEntity>(this IFilterSpecification<TEntity> specification,
                                                                                 Expression<Func<TEntity, string>> propertySelector,
                                                                                 TextFilter firstFilter,
                                                                                 string firstValue,
                                                                                 FilterLogicOperation operation,
                                                                                 TextFilter secondFilter,
                                                                                 string secondValue)
                where TEntity : IEntity
        {
            var expression = TextFilterExpressionHelper.GetDoubleFilter(propertySelector, firstFilter, firstValue, operation, secondFilter, secondValue);

            specification.Filters.Add(expression);

            return specification;
        }

        /// <inheritdoc />
        public static IFilterSpecification<TEntity> AddNumberFilter<TEntity>(this IFilterSpecification<TEntity> specification,
                                                                             Expression<Func<TEntity, decimal>> propertySelector,
                                                                             NumberFilter filter,
                                                                             decimal value)
                where TEntity : IEntity
        {
            var expression = NumberFilterExpressionHelper<decimal>.GetFilter(propertySelector, filter, value);

            specification.Filters.Add(expression);

            return specification;
        }

        /// <inheritdoc />
        public static IFilterSpecification<TEntity> AddNumberDoubleFilter<TEntity>(this IFilterSpecification<TEntity> specification,
                                                                                   Expression<Func<TEntity, decimal>> propertySelector,
                                                                                   NumberFilter firstFilter,
                                                                                   decimal firstValue,
                                                                                   FilterLogicOperation operation,
                                                                                   NumberFilter secondFilter,
                                                                                   decimal secondValue)
                where TEntity : IEntity
        {
            var expression = NumberFilterExpressionHelper<decimal>.GetDoubleFilter(propertySelector, firstFilter, firstValue, operation, secondFilter, secondValue);

            specification.Filters.Add(expression);

            return specification;
        }

        /// <inheritdoc />
        public static IFilterSpecification<TEntity> AddNumberFilter<TEntity>(this IFilterSpecification<TEntity> specification, Expression<Func<TEntity, int>> propertySelector, NumberFilter filter, int value)
                where TEntity : IEntity
        {
            var expression = NumberFilterExpressionHelper<int>.GetFilter(propertySelector, filter, value);

            specification.Filters.Add(expression);

            return specification;
        }

        /// <inheritdoc />
        public static IFilterSpecification<TEntity> AddNumberDoubleFilter<TEntity>(this IFilterSpecification<TEntity> specification,
                                                                                   Expression<Func<TEntity, int>> propertySelector,
                                                                                   NumberFilter firstFilter,
                                                                                   int firstValue,
                                                                                   FilterLogicOperation operation,
                                                                                   NumberFilter secondFilter,
                                                                                   int secondValue)
                where TEntity : IEntity
        {
            var expression = NumberFilterExpressionHelper<int>.GetDoubleFilter(propertySelector, firstFilter, firstValue, operation, secondFilter, secondValue);

            specification.Filters.Add(expression);

            return specification;
        }
    }
}