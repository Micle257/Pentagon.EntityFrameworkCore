// -----------------------------------------------------------------------
//  <copyright file="INumberCompositeFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using Entities;
    using EntityFrameworkCore.Specifications;

    public interface INumberCompositeFilterBuilder<TEntity> : ISpecificationFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        ISpecificationFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value);

        ISpecificationFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter);
    }
}