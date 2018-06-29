// -----------------------------------------------------------------------
//  <copyright file="IOrderSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Entities;

    /// <summary> Represents a entity specification for query pipeline, that is capable of specifing the order of entities. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IOrderSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets a value indicating whether the order is descending. </summary>
        /// <value> <c> true </c> if the order is descending; otherwise, <c> false </c>. </value>
        bool IsDescending { get; }

        /// <summary> Gets the order function for specification. </summary>
        /// <value> The expression tree of the criteria. </value>
        Expression<Func<TEntity, object>> Order { get; }
    }
}