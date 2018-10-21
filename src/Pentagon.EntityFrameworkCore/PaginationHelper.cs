﻿// -----------------------------------------------------------------------
//  <copyright file="PaginationService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using Collections;
    using Microsoft.EntityFrameworkCore;

    public static class PaginationHelper
    {
        public static async Task<PagedList<TSelectEntity>> CreateAsync<TSelectEntity, TEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                        IQueryable<TEntity> query,
                                                                                        IPaginationSpecification<TEntity> specification)
                where TEntity : IEntity
        {
            var count = await query.CountAsync().ConfigureAwait(false);

            var possiblePageCount = count / specification.PageSize + 1;

            if (count < specification.PageSize)
                possiblePageCount = 1;

            // If page index is overflowed.
            if (specification.PageNumber > possiblePageCount + 1)
                throw new ArgumentOutOfRangeException(nameof(specification.PageNumber), message: "The page number is out of range.");

            query = specification.ApplyPagination(query);
            var list = await query.Select(selector).ToListAsync().ConfigureAwait(false);
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