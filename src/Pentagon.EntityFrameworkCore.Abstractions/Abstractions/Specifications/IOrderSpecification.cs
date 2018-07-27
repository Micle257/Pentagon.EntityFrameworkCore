// -----------------------------------------------------------------------
//  <copyright file="IOrderSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System.Collections.Generic;
    using Entities;

    /// <summary> Represents a entity specification for query pipeline, that is capable of specifing the order of entities. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IOrderSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the list of order specifications. </summary>
        /// <value> The list of orders. </value>
        IList<SpecificationOrder<TEntity>> Orders { get; }
    }
}