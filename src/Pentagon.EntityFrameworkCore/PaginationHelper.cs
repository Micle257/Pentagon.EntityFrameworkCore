// -----------------------------------------------------------------------
//  <copyright file="PaginationService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Collections;
    using Exceptions;
    using Interfaces.Entities;
    using Interfaces.Specifications;
    using Microsoft.EntityFrameworkCore;
    using Specifications;

    public static class PaginationHelper
    {
        public static async Task<PagedList<TSelectEntity>> CreateAsync<TSelectEntity, TEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                        IQueryable<TEntity> query,
                                                                                        IPaginationSpecification<TEntity> specification)
                where TEntity : IEntity
        {

            // query count of all items under filter
            var count = await query.CountAsync().ConfigureAwait(false);

            // create blank paged list for computation
            var blankPagedList = PagedList<TSelectEntity>.CreateBlank(specification.PageSize, count, specification.PageSize, specification.PageNumber - 1);

            // if no data...
            if (count == 0)
            {
                // return blank paged list
                return blankPagedList;
            }

            if (specification.PageNumber > blankPagedList.TotalPages)
                throw new PageOutOfRangeException(nameof(specification.PageNumber), specification.PageNumber, blankPagedList.TotalPages);

            query = specification.ApplyPagination(query);

            var list = await query.Select(selector).ToListAsync().ConfigureAwait(false);

            return new PagedList<TSelectEntity>(list, count, specification.PageSize, specification.PageNumber - 1);
        }

        public static PagedList<TSelectEntity> Create<TSelectEntity, TEntity>(Func<TEntity, TSelectEntity> selector,
                                                                                               IEnumerable<TEntity> queryIteration,
                                                                                               IPaginationSpecification<TEntity> specification)
                where TEntity : IEntity
        {
            var query = queryIteration.ToList();

            var count = query.Count;

            var possiblePageCount = count / specification.PageSize + 1;

            if (count < specification.PageSize)
                possiblePageCount = 1;

            // If page index is overflowed.
            if (specification.PageNumber > possiblePageCount + 1)
                throw new ArgumentOutOfRangeException(nameof(specification.PageNumber), message: "The page number is out of range.");

            query = SpecificationHelper.ApplyPagination(query, specification).ToList();

            var list = query.Select(selector);

            return new PagedList<TSelectEntity>(list, count, specification.PageSize, specification.PageNumber - 1);
        }

        public static async Task<PagedList<TEntity>> CreateAsync<TEntity>(IQueryable<TEntity> query, IPaginationSpecification<TEntity> specification)
                where TEntity : IEntity
        {
            var count = await query.CountAsync().ConfigureAwait(false);

            var possiblePageCount = count / specification.PageSize + 1;

            if (specification.PageNumber > possiblePageCount + 1)
                throw new ArgumentOutOfRangeException(nameof(specification.PageNumber), message: "The page number is out of range.");

            var list = await specification.ApplyPagination(query).ToListAsync().ConfigureAwait(false);

            return new PagedList<TEntity>(list, count, specification.PageSize, specification.PageNumber - 1);
        }
    }
}