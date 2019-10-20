// -----------------------------------------------------------------------
//  <copyright file="IConnectedCompositeTextFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Filters
{
    using Entities;
    using EntityFrameworkCore.Filters;

    public interface IConnectedCompositeTextFilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null);

        IFilterBuilder<TEntity> AddOr(TextFilter filter, string value = null);

        IFilterBuilder<TEntity> AddAnd(TextFilter filter, string value = null);
    }
}