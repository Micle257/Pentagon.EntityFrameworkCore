// -----------------------------------------------------------------------
//  <copyright file="IConnectedCompositeNumberFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Filters
{
    using Entities;
    using EntityFrameworkCore.Filters;

    public interface IConnectedCompositeNumberFilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null);

        IFilterBuilder<TEntity> AddOr(NumberFilter filter, object value = null);

        IFilterBuilder<TEntity> AddAnd( NumberFilter filter, object value = null);
    }
}