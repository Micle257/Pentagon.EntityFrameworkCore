// -----------------------------------------------------------------------
//  <copyright file="PageManager.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Collections.Generic;
    using Abstractions;

    /// <summary> Represents a manager for managing paging for a collection. </summary>
    /// <typeparam name="TEntity"> The type of the element. </typeparam>
    public class PagedList<TEntity> : IPagedList<TEntity>
    {
        /// <summary> Initializes a new instance of the <see cref="PagedList{T}" /> class. </summary>
        /// <param name="source"> The source collection. </param>
        /// <param name="pageSize"> Size of the page. </param>
        /// <param name="indexFrom"> The index from. </param>
        public PagedList(IEnumerable<TEntity> source, int totalCount, int pageSize, int pageIndex)
        {
            Items = source;
            TotalCount = totalCount;
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalPages = (int) (TotalCount / (double) PageSize);
        }

        /// <inheritdoc />
        public int PageSize { get; }

        /// <inheritdoc />
        public int PageIndex { get; }

        /// <inheritdoc />
        public int TotalCount { get; }
        
        /// <inheritdoc />
        public int TotalPages { get; }

        /// <summary> Gets the source collection. </summary>
        /// <value> The <see cref="IEnumerable{T}" />. </value>
        public IEnumerable<TEntity> Items { get; }

        /// <inheritdoc />
        public bool HasPreviousPage => PageIndex <= TotalPages;

        /// <inheritdoc />
        public bool HasNextPage => PageIndex > TotalPages;
    }
}