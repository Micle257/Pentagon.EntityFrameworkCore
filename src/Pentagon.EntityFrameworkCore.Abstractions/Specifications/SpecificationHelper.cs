// -----------------------------------------------------------------------
//  <copyright file="SpecificationHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces.Entities;
    using Interfaces.Specifications;
    using JetBrains.Annotations;

    public static class SpecificationHelper
    {
        /// <summary> Applies the specification to the collection. </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="specification">The specification.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">collection</exception>
        [NotNull]
        [Pure]
        public static IEnumerable<TEntity> Apply<TEntity>([NotNull] IEnumerable<TEntity> collection, [NotNull] ISpecification<TEntity> specification)
                where TEntity : IEntity
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var query = collection.AsQueryable();

            query = specification.Apply(query: query);

            if (specification is IPaginationSpecification<TEntity> pagination)
                query = pagination.ApplyPagination(query: query);

            return query.AsEnumerable();
        }
    }
}