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
        public static IFilterSpecification<TEntity> AddNumberFilter<TEntity, TValue>(this IFilterSpecification<TEntity> specification,
                                                                             Expression<Func<TEntity, TValue>> propertySelector,
                                                                             NumberFilter filter,
                                                                                     TValue value)
                where TEntity : IEntity
        {
            var expression = NumberFilterExpressionHelper<TValue>.GetFilter(propertySelector, filter, value);

            specification.Filters.Add(expression);

            return specification;
        }

        /// <inheritdoc />
        public static IFilterSpecification<TEntity> AddNumberDoubleFilter<TEntity, TValue>(this IFilterSpecification<TEntity> specification,
                                                                                   Expression<Func<TEntity, TValue>> propertySelector,
                                                                                   NumberFilter firstFilter,
                                                                                           TValue firstValue,
                                                                                   FilterLogicOperation operation,
                                                                                   NumberFilter secondFilter,
                                                                                           TValue secondValue)
                where TEntity : IEntity
        {
            var expression = NumberFilterExpressionHelper<TValue>.GetDoubleFilter(propertySelector, firstFilter, firstValue, operation, secondFilter, secondValue);

            specification.Filters.Add(expression);

            return specification;
        }
    }
}