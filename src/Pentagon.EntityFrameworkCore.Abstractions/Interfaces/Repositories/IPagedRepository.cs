// -----------------------------------------------------------------------
//  <copyright file="IPagedRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Repositories
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using Specifications;

    public interface IPagedRepository<TEntity>
            where TEntity : IEntity
    {
        Task<PagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>;

        Task<PagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex, CancellationToken cancellationToken = default);
    }
}