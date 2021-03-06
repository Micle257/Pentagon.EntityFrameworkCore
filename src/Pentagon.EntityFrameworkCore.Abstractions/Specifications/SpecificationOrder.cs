// -----------------------------------------------------------------------
//  <copyright file="SpecificationOrder.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Interfaces.Entities;

    /// <summary> Represents a pair of order specification. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class SpecificationOrder<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets a value indicating whether the order is descending. </summary>
        /// <value> <c> true </c> if the order is descending; <c> false </c> if the order is ascending. </value>
       public bool IsDescending { get; internal set;  }

        /// <summary> Gets the order function for specification. </summary>
        /// <value> The expression tree of the criteria. </value>
        public  Expression<Func<TEntity, object>> Order { get; internal set; }
    }
}