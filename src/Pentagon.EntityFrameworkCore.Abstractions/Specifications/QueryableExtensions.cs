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
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using Collections;
    using Helpers;
    using JetBrains.Annotations;

    public static class QueryableExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>([NotNull] this IQueryable<TEntity> query, Expression<Func<TEntity, object>> order = null, bool isDescending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            order = order ?? (e => e);

            var orderedQuery = isDescending ? query.OrderByDescending(order) : query.OrderBy(order);

            return orderedQuery;
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>([NotNull] this IOrderedQueryable<TEntity> query, Expression<Func<TEntity, object>> order = null, bool isDescending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            order = order ?? (e => e);

            var orderedQuery = isDescending ? query.ThenByDescending(order) : query.ThenBy(order);

            return orderedQuery;
        }

        public static IQueryable<TEntity> Filter<TEntity>([NotNull] this IQueryable<TEntity> query, Action<IFilterBuilder<TEntity>> configure = null)
                where TEntity : IEntity
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var builder = new FilterBuilder<TEntity>();

            configure?.Invoke(builder);

            var filters = builder.BuildFilter();

            if (filters != null)
                query = query.Where(filters);

            return query;
        }

        public static IQueryable<TEntity> Filter<TEntity>([NotNull] this IQueryable<TEntity> query, Action<IStartedPredicateBuilder<TEntity>> configure = null)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var builder = new PredicateBuilder<TEntity>();

            configure?.Invoke(builder);

            var filter = builder.Build();

            if (filter != null)
                query = query.Where(filter);

            return query;
        }

        public static IPagedQueryable<TEntity> Page<TEntity>([NotNull] this IQueryable<TEntity> query, int pageNumber, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var parameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (!parameters.AreValid)
                throw new InvalidPaginationParametersException(parameters);

            if (parameters?.AreValid == true)
                query = query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);

            return new PagedQueryable<TEntity>(query, parameters);
        }

        public static IPagedQueryable<TEntity> Page<TEntity>([NotNull] this IQueryable<TEntity> query, PaginationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (parameters?.AreValid != true)
                throw new InvalidPaginationParametersException(parameters);

            query = query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);

            return new PagedQueryable<TEntity>(query, parameters);
        }
    }
}