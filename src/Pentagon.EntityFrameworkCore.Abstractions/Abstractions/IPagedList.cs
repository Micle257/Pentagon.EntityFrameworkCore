namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Collections.Generic;

    public interface IPagedList<TEntity>

    {
        /// <summary> Gets the source collection. </summary>
        /// <value> The <see cref="IEnumerable{TEntity}" />. </value>
        IEnumerable<TEntity> Items { get; }

        /// <summary> Gets the element count of the page. </summary>
        /// <value> The <see cref="int" />. </value>
        int PageSize { get; }

        /// <summary> Gets the total count of elements. </summary>
        /// <value> The <see cref="int" />. </value>
        int TotalCount { get; }

        /// <summary> Gets the page index. </summary>
        /// <value> The <see cref="int" />. </value>
        int PageIndex { get; }

        /// <summary> Gets the total count of pages. </summary>
        /// <value> The <see cref="int" />. </value>
        int TotalPages { get; }

        /// <summary> Gets a value indicating whether this page has previous page. </summary>
        /// <value>
        ///     <c> true </c> if this page has previous page; otherwise, <c> false </c>.
        /// </value>
        bool HasPreviousPage { get; }

        /// <summary> Gets a value indicating whether this page has next page. </summary>
        /// <value>
        ///     <c> true </c> if this page has next page; otherwise, <c> false </c>.
        /// </value>
        bool HasNextPage { get; }
    }
}