// -----------------------------------------------------------------------
//  <copyright file="IAllPaginationSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using Entities;

    /// <summary> Represents an entity specification for query pipeline, that is capable of pagination all pages. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IAllPaginationSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the entities count per page. </summary>
        /// <value> The <see cref="int" />. </value>
        int PageSize { get; }
    }
}