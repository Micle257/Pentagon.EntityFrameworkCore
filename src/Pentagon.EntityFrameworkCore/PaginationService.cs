// -----------------------------------------------------------------------
//  <copyright file="PaginationService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.Data.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Pentagon.Extensions.DependencyInjection;

    [Register(RegisterType.Singleton, typeof(IPaginationService))]
    public class PaginationService : IPaginationService
    {
        /// <inheritdoc />
        public async Task<IPagedList<TEntity>> CreateAsync<TEntity>(IQueryable<TEntity> query, IPaginationSpecification<TEntity> specification)
            where TEntity : IEntity
        {
            var count = await query.CountAsync().ConfigureAwait(false);
            var list = await specification.ApplyPagination(query).ToListAsync().ConfigureAwait(false);
            return new PagedList<TEntity>(list, count, specification.PageSize, specification.PageIndex);
        }

        /// <inheritdoc />
        public async Task<IPagedList<TSelectEntity>> CreateAsync<TSelectEntity, TEntity>(Expression<Func<TEntity, TSelectEntity>> selector, IQueryable<TEntity> query, IPaginationSpecification<TEntity> specification)
                where TEntity : IEntity
        {
            var count = await query.CountAsync().ConfigureAwait(false);
            query = specification.ApplyPagination(query);
            var list = await query.Select(selector).ToListAsync().ConfigureAwait(false);
            return new PagedList<TSelectEntity>(list, count, specification.PageSize, specification.PageIndex);
        }
    }
}