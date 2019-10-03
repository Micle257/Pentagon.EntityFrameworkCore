// -----------------------------------------------------------------------
//  <copyright file="FilterSpecificationExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Filters;
    using Helpers;
    using Interfaces.Entities;
    using Interfaces.Filters;
    using Interfaces.Specifications;

    public static class FilterSpecificationExtensions
    {
        public static IFilterSpecification<TEntity> AddFilter<TEntity>(this IFilterSpecification<TEntity> specification, Action<IFilterBuilder<TEntity>> configure)
                where TEntity : IEntity
        {
            var builder = new FilterBuilder<TEntity>();

            configure(builder);

            if (!builder.HasAnyFilter)
                return specification;

            var filters = builder.BuildFilter();

            specification.Filters.Add(filters);

            return specification;
        }

        public static IFilterSpecification<TEntity> AddFilter<TEntity>(this IFilterSpecification<TEntity> specification, Action<IStartedPredicateBuilder<TEntity>> configure)
                where TEntity : IEntity
        {
            var builder = new PredicateBuilder<TEntity>();

            configure(builder);

            specification.Filters.Add(builder.Build());

            return specification;
        }

        public static IFilterSpecification<TEntity> AddFilter<TEntity>(this IFilterSpecification<TEntity> specification, Expression<Func<TEntity,bool>> selector)
                where TEntity : IEntity
        {
            specification.Filters.Add(selector);

            return specification;
        }
    }
}