// -----------------------------------------------------------------------
//  <copyright file="GetOneSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using JetBrains.Annotations;

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for get one operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetOneSpecification<TEntity> : IFilterSpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Initializes a new instance of the <see cref="GetOneSpecification{TEntity}" /> class. </summary>
        /// <param name="filter"> The filter. </param>
        public GetOneSpecification([NotNull] Expression<Func<TEntity, bool>> filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> Filter { get; }

        /// <inheritdoc />
        public IList<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

        /// <inheritdoc />
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            if (Filter != null)
                query = query.Where(Filter);

            return query;
        }
    }
}