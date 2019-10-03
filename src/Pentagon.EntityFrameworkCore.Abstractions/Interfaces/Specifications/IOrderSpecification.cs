// -----------------------------------------------------------------------
//  <copyright file="IOrderSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications;

    /// <summary> Represents a entity specification for query pipeline, that is capable of specifing the order of entities. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IOrderSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the list of order specifications. </summary>
        /// <value> The list of orders. </value>
        IReadOnlyList<SpecificationOrder<TEntity>> Orders { get; }

        /// <summary> Adds the order specification. </summary>
        /// <param name="order"> The order. </param>
        /// <param name="isDescending"> if set to <c> true </c> is descending. </param>
        /// <returns> Calling object (this) for fluent API. </returns>
        IOrderSpecification<TEntity> AddOrder(Expression<Func<TEntity, object>> order, bool isDescending);
    }
}