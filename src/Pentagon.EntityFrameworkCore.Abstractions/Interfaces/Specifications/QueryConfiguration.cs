// -----------------------------------------------------------------------
//  <copyright file="QueryConfiguration.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;

    public class QueryConfiguration<TEntity> : IQueryConfiguration<TEntity>
            where TEntity : IEntity
    {
        public QueryConfiguration() { }

        public QueryConfiguration(params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] configurations)
        {
            foreach (var configuration in configurations)
                QueryConfigurations.Add(configuration);
        }

        /// <inheritdoc />
        public List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> QueryConfigurations { get; } = new List<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();
    }
}