// -----------------------------------------------------------------------
//  <copyright file="ITextCompositeFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using Entities;
    using EntityFrameworkCore.Specifications;

    public interface ITextCompositeFilterBuilder<TEntity> : ISpecificationFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        ISpecificationFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null);
    }
}