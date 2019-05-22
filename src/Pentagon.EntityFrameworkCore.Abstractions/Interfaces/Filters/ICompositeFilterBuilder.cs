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
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition);
    }

    public interface IConnectedCompositeFilterBuilder<TEntity> : ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null);
        
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null);
    }
}