// -----------------------------------------------------------------------
//  <copyright file="IPagedRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using Specifications;

    public interface IPagedRepository<TEntity>
            where TEntity : IEntity
    {
        Task<PagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>;

        Task<PagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex);
    }
}