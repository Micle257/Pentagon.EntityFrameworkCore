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
    using EntityFrameworkCore.Specifications.Filters;

    public interface ICompositeFilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition);
    }

    public interface ITextCompositeFilterBuilder<TEntity> : ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        ITextCompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null);
    }

    public interface INumberCompositeFilterBuilder<TEntity> : ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        INumberCompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null);
    }
}