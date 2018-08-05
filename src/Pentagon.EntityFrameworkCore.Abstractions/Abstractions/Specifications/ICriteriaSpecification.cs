// -----------------------------------------------------------------------
//  <copyright file="ICriteriaSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications;

    /// <summary> Represents an entity specification for query, adding filter condition for returned entities (WHERE in T-SQL). </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IFilterSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the filter function collection. </summary>
        /// <value> The list of filters. </value>
        ICollection<Expression<Func<TEntity, bool>>> Filters { get; }
    }
}