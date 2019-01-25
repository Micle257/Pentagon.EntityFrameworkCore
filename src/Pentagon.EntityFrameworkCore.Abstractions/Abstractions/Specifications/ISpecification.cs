// -----------------------------------------------------------------------
//  <copyright file="ISpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;

    /// <summary> Represents a entity specification for query pipeline. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the query configurations. </summary>
        /// <value> The query configurations. </value>
        List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> QueryConfigurations { get; }

        /// <summary> Applies the specification to the query. </summary>
        /// <param name="query"> The query. </param>
        /// <returns> A modified query. </returns>
        IQueryable<TEntity> Apply(IQueryable<TEntity> query);
    }
}