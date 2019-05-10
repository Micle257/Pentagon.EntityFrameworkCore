// -----------------------------------------------------------------------
//  <copyright file="INumberCompositeFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using Entities;
    using EntityFrameworkCore.Specifications;
    using EntityFrameworkCore.Specifications.Filters;

    public interface INumberCompositeFilterBuilder<TEntity> : ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value);

        ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter);
    }
}