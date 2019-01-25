// -----------------------------------------------------------------------
//  <copyright file="EmptyEntityIncludeConfiguration.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Linq;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class EmptyEntityIncludeConfiguration<TEntity> : IEntityIncludeConfiguration<TEntity>
            where TEntity : IEntity
    {
        /// <inheritdoc />
        public IQueryable<TEntity> Configure(IQueryable<TEntity> specification) => specification;
    }
}