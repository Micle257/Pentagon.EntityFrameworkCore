// -----------------------------------------------------------------------
//  <copyright file="QueryableExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Filters;
    using Helpers;
    using Interfaces.Entities;
    using Interfaces.Filters;
    using JetBrains.Annotations;

    public static class QueryableExtensions
    {
        public static IOrderedQueryable<TEntity> SortBy<TEntity>([NotNull] this IQueryable<TEntity> query, Expression<Func<TEntity, object>> order = null, bool isDescending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            order = order ?? (e => e);
            
            return isDescending ? query.OrderByDescending(order) : query.OrderBy(order);
        }

        public static IOrderedQueryable<TEntity> ThenSortBy<TEntity>([NotNull] this IOrderedQueryable<TEntity> query, Expression<Func<TEntity, object>> order = null, bool isDescending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            order = order ?? (e => e);

            return isDescending ? query.ThenByDescending(order) : query.ThenBy(order);
        }

        public static IQueryable<TEntity> Filter<TEntity>([NotNull] this IQueryable<TEntity> query, Action<IFilterBuilder<TEntity>> configure = null)
                where TEntity : IEntity
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var builder = new FilterBuilder<TEntity>();

            configure?.Invoke(builder);

            var filters = builder.BuildFilter();

            return filters != null ? query.Where(filters) : query;
        }

        public static IQueryable<TEntity> Filter<TEntity>([NotNull] this IQueryable<TEntity> query, Action<IStartedPredicateBuilder<TEntity>> configure = null)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var builder = new PredicateBuilder<TEntity>();

            configure?.Invoke(builder);

            var filter = builder.Build();

            return filter != null ? query.Where(filter) : query;
        }
    }
}