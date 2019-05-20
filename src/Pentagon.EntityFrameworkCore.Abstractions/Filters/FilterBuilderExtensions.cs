namespace Pentagon.EntityFrameworkCore.Specifications.Filters {
    using System;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public static class FilterBuilderExtensions
    {
        /// <inheritdoc />
        public static IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value = null)
                where TEntity : IEntity
        {
            return that.AddCompositeFilter(FilterExpressionHelper.GetObjectPropertySelector(propertySelector), FilterExpressionHelper.GetTextFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, int>> propertySelector, NumberFilter filter, int value)
                where TEntity : IEntity
        {
            return that.AddCompositeFilter(FilterExpressionHelper.GetObjectPropertySelector(propertySelector), FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, int?>> propertySelector, NumberFilter filter, int? value = null)
                where TEntity : IEntity
        {
            return that.AddCompositeFilter(FilterExpressionHelper.GetObjectPropertySelector(propertySelector), FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, decimal>> propertySelector, NumberFilter filter, decimal value)
                where TEntity : IEntity
        {
            return that.AddCompositeFilter(FilterExpressionHelper.GetObjectPropertySelector(propertySelector), FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, decimal?>> propertySelector, NumberFilter filter, decimal? value = null)
                where TEntity : IEntity
        {
            return that.AddCompositeFilter(FilterExpressionHelper.GetObjectPropertySelector(propertySelector), FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }
    }
}