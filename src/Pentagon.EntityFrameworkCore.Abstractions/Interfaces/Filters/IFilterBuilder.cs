// -----------------------------------------------------------------------
//  <copyright file="IFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications.Filters;

    public interface IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        bool HasAnyFilter { get; }

        FilterLogicOperation ValueFilterConcatOperation { get; set; }

        /// <summary> Gets or sets the filter concat operation. Represented as OR / AND. Default is AND. </summary>
        /// <value> The <see cref="FilterLogicOperation" />. </value>
        FilterLogicOperation FilterConcatOperation { get; set; }

        IFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values);

        ICompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, bool>> condition);

        IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, Expression<Func<TEntity, bool>> condition);

        IFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition);

        Expression<Func<TEntity, bool>> BuildFilter();
    }
}