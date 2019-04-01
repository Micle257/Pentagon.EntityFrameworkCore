// -----------------------------------------------------------------------
//  <copyright file="ITextCompositeFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using Entities;
    using EntityFrameworkCore.Specifications;
    using EntityFrameworkCore.Specifications.Filters;

    public interface ITextCompositeFilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null);
    }
}