﻿// -----------------------------------------------------------------------
//  <copyright file="IFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Filters;
    using JetBrains.Annotations;

    public interface IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        [NotNull]
        IReadOnlyList<Expression<Func<TEntity, bool>>> Filters { get; }

        [NotNull]
        IReadOnlyList<ICompositeFilter> CompositeFilters { get; }

        [NotNull]
        IReadOnlyList<Expression<Func<TEntity, bool>>> ValueFilters { get; }

        bool HasAnyFilter { get; }

        FilterLogicOperation ValueFilterConcatOperation { get; set; }

        /// <summary> Gets or sets the filter concat operation. Represented as OR / AND. Default is AND. </summary>
        /// <value> The <see cref="FilterLogicOperation" />. </value>
        FilterLogicOperation FilterConcatOperation { get; set; }

        IFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values);

        ICompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, bool>> condition);

        ICompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> condition);

        IFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition);

        Expression<Func<TEntity, bool>> BuildFilter();
    }
}