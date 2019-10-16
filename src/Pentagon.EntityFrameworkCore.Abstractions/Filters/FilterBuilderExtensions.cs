namespace Pentagon.EntityFrameworkCore.Filters {
    using System;
    using System.Linq.Expressions;
    using Interfaces.Entities;
    using Interfaces.Filters;

    public static class FilterBuilderExtensions
    {
        /// <inheritdoc />
        public static IConnectedCompositeTextFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value = null)
                where TEntity : IEntity
        {
            return (IConnectedCompositeTextFilterBuilder<TEntity>) that.AddCompositeFilter(FilterExpressionHelper.GetTextFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeNumberFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, int>> propertySelector, NumberFilter filter, int value)
                where TEntity : IEntity
        {
            return (IConnectedCompositeNumberFilterBuilder<TEntity>) that.AddCompositeFilter(FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeNumberFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, int?>> propertySelector, NumberFilter filter, int? value = null)
                where TEntity : IEntity
        {
            return (IConnectedCompositeNumberFilterBuilder<TEntity>)that.AddCompositeFilter(FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeNumberFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, decimal>> propertySelector, NumberFilter filter, decimal value)
                where TEntity : IEntity
        {
            return (IConnectedCompositeNumberFilterBuilder<TEntity>)that.AddCompositeFilter(FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }

        /// <inheritdoc />
        public static IConnectedCompositeNumberFilterBuilder<TEntity> AddCompositeFilter<TEntity>(this IFilterBuilder<TEntity> that, Expression<Func<TEntity, decimal?>> propertySelector, NumberFilter filter, decimal? value = null)
                where TEntity : IEntity
        {
            return (IConnectedCompositeNumberFilterBuilder<TEntity>)that.AddCompositeFilter(FilterExpressionHelper.GetNumberFilterCallback(propertySelector, filter, value));
        }
    }
}