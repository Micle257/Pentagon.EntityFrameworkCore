﻿// -----------------------------------------------------------------------
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
    using Interfaces.Entities;
    using Interfaces.Specifications;
    using JetBrains.Annotations;

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for get one operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetOneSpecification<TEntity> : IFilterSpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Initializes a new instance of the <see cref="GetOneSpecification{TEntity}" /> class. </summary>
        /// <param name="filter"> The filter. </param>
        public GetOneSpecification(Expression<Func<TEntity, bool>> filter = null)
        {
            if (filter != null)
                Filters.Add(filter);
        }

        /// <inheritdoc />
        [NotNull]
        public List<Expression<Func<TEntity, bool>>> Filters { get; } = new List<Expression<Func<TEntity, bool>>>();

        /// <inheritdoc />
        public List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> QueryConfigurations { get; } = new List<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();

        /// <inheritdoc />
        public IQueryable<TEntity> Apply([NotNull] IQueryable<TEntity> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));


            foreach (var configuration in QueryConfigurations)
            {
                query = configuration(query);
            }
            
            if (Filters.Count == 0)
                return query;

            foreach (var filter in Filters)
            {
                query = query.Where(filter);
            }

            return query;
        }
    }
}