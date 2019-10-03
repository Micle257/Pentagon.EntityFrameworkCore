// -----------------------------------------------------------------------
//  <copyright file="IPaginationSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Specifications
{
    using System.Linq;
    using Entities;

    /// <summary> Represents an entity specification for query pipeline, that is capable of pagination. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IPaginationSpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets or sets the size of the page. </summary>
        /// <value> The <see cref="int" />. </value>
        int PageSize { get; set; }

        /// <summary> Gets the index of the page. </summary>
        /// <value> The <see cref="int" />. </value>
        int PageNumber { get; set; }

        /// <summary> Applies the pagination specification to the query. </summary>
        /// <param name="query"> The query. </param>
        /// <returns> A modified query. </returns>
        IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query);
    }
}