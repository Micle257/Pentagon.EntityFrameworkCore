// -----------------------------------------------------------------------
//  <copyright file="PaginationService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.Data.EntityFramework
{
    using System.Collections.Generic;
    using System.Linq;
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
            var specQuery = specification.Apply(query);
            var count = await specQuery.CountAsync().ConfigureAwait(false);
            var list = await specification.ApplyPagination(specQuery).ToListAsync().ConfigureAwait(false);
            return new PagedList<TEntity>(list, count, specification.PageSize, specification.PageIndex);
        }
    }
}