// -----------------------------------------------------------------------
//  <copyright file="ISpecificationFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications;

    public interface ISpecificationFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        ISpecificationFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values);

        ITextCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value = null);

        INumberCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, NumberFilter filter, object value);

        INumberCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, NumberFilter filter);

        ISpecificationFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition);

        FilterLogicOperation ValueFilterConcatOperation { get; set; }

        Expression<Func<TEntity, bool>> BuildFilter();
    }
}