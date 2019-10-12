// -----------------------------------------------------------------------
//  <copyright file="QueryableExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Collections;
    using Exceptions;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Specifications;

    public static class QueryableExtensions
    {
        public static Task<PagedList<TEntity>> ToPagedListAsync<TEntity>([NotNull] this IQueryable<TEntity> query, int pageNumber, int pageSize)
        {
            return ToPagedListAsync(query, new PaginationParameters {PageSize = pageSize, PageNumber = pageNumber});
        }

        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity>([NotNull] this IQueryable<TEntity> query, [NotNull] PaginationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.AreValid == false)
                throw new InvalidPaginationParametersException(parameters);

            // query count of all items under filter
            var count = await query.CountAsync().ConfigureAwait(false);

            // create blank paged list for computation
            var blankPagedList = PagedList<TEntity>.CreateBlank(parameters.PageSize, count, parameters.PageSize, parameters.PageNumber - 1);

            Debug.Assert(blankPagedList != null, nameof(blankPagedList) + " != null");

            if (parameters.PageNumber > blankPagedList.TotalPages)
                throw new PageOutOfRangeException(nameof(parameters.PageNumber), parameters.PageNumber, blankPagedList.TotalPages);
            
            query = query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);

            var list = await query.ToListAsync().ConfigureAwait(false);

            return new PagedList<TEntity>(list, count, parameters.PageSize, parameters.PageNumber - 1);
        }
        
        public static async Task<int> CountPagesAsync<TEntity>([NotNull] this IQueryable<TEntity> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (pageSize < 1)
                throw new InvalidPaginationParametersException(new PaginationParameters {  PageSize = pageSize});
            
            var count = await query.CountAsync().ConfigureAwait(false);

            return count / pageSize + 1;
        }

        public static async Task<int> CountPagesAsync<TEntity>([NotNull] this IQueryable<TEntity> query, PaginationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (parameters?.AreValid == false)
                throw new InvalidPaginationParametersException(parameters);

            var count = await query.CountAsync().ConfigureAwait(false);

            return count / parameters.PageSize + 1;
        }
    }
}