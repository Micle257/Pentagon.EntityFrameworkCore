// -----------------------------------------------------------------------
//  <copyright file="QuerySpecificationExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq;
    using Interfaces.Entities;
    using Interfaces.Specifications;

    public static class QuerySpecificationExtensions
    {
        public static IQueryConfiguration<TEntity> AddConfiguration<TEntity>(this IQueryConfiguration<TEntity> queryConfiguration, Func<IQueryable<TEntity>, IQueryable<TEntity>> configure)
                where TEntity : IEntity
        {
            queryConfiguration.QueryConfigurations.Add(configure);

            return queryConfiguration;
        }

        public static IQueryConfiguration<TEntity> AddConfiguration<TEntity>(this IQueryConfiguration<TEntity> queryConfiguration, IQueryConfiguration<TEntity> configure)
                where TEntity : IEntity
        {
            foreach (var configuration in configure.QueryConfigurations)
            {
                queryConfiguration.QueryConfigurations.Add(configuration);
            }

            return queryConfiguration;
        }
    }
}