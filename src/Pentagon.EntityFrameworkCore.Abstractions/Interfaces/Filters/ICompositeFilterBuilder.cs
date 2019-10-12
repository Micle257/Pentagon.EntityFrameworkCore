// -----------------------------------------------------------------------
//  <copyright file="ISpecificationFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Filters
{
    using System;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Filters;

    public interface ICompositeFilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition);
    }
}