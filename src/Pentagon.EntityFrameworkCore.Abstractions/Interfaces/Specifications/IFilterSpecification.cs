// -----------------------------------------------------------------------
//  <copyright file="ICriteriaSpecification.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Entities;

    /// <summary> Represents an entity specification for query, adding filter condition for returned entities (WHERE in T-SQL). </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IFilterSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the filter function collection. </summary>
        /// <value> The list of filters. </value>
        List<Expression<Func<TEntity, bool>>> Filters { get; }
    }
}