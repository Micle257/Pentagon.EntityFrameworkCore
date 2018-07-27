// -----------------------------------------------------------------------
//  <copyright file="ICriteriaSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Entities;

    /// <summary> Represents an entity specification for query, adding filter condition for returned entities (WHERE in T-SQL). </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IFilterSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the criteria function for specification. </summary>
        /// <value> The expression tree of the criteria. </value>
        Expression<Func<TEntity, bool>> Filter { get; }
    }
}