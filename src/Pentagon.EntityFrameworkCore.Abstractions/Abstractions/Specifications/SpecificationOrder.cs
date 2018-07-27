// -----------------------------------------------------------------------
//  <copyright file="SpecificationOrder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Entities;

    /// <summary> Represents a pair of order specification. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class SpecificationOrder<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets a value indicating whether the order is descending. </summary>
        /// <value> <c> true </c> if the order is descending; <c> false </c> if the order is ascending. </value>
        bool IsDescending { get; }

        /// <summary> Gets the order function for specification. </summary>
        /// <value> The expression tree of the criteria. </value>
        Expression<Func<TEntity, object>> Order { get; }
    }
}