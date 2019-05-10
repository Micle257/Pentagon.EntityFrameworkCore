namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications {
    using System;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications.Filters;

    public interface IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values);

        ICompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, bool>> condition);

        ITextCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value = null);

        INumberCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, NumberFilter filter, object value = null);

        IFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition);

        FilterLogicOperation ValueFilterConcatOperation { get; set; }

        Expression<Func<TEntity, bool>> BuildFilter();
    }
}