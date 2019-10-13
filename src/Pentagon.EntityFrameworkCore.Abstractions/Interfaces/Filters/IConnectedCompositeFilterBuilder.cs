﻿// -----------------------------------------------------------------------
//  <copyright file="IConnectedCompositeFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Filters
{
    using Entities;
    using EntityFrameworkCore.Filters;

    public interface IConnectedCompositeFilterBuilder<TEntity> : IConnectedCompositeNumberFilterBuilder<TEntity>, IConnectedCompositeTextFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null);
    }
}