// -----------------------------------------------------------------------
//  <copyright file="IPaginationService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using Specifications;

    public interface IPaginationService
    {
        Task<PagedList<TSelectEntity>> CreateAsync<TSelectEntity, TEntity>(Expression<Func<TEntity, TSelectEntity>> selector, IQueryable<TEntity> query, IPaginationSpecification<TEntity> specification)
                where TEntity : IEntity;
    }
}