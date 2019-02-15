// -----------------------------------------------------------------------
//  <copyright file="Specifications.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using JetBrains.Annotations;

    public static class Specifications
    {
        public static GetManySpecification<TEntity> Many<TEntity>([NotNull] Expression<Func<TEntity, bool>> filter)
                where TEntity : IEntity
        {
            var spec = new GetManySpecification<TEntity>();

            spec.AddFilter(filter);

            return spec;
        }

        public static GetManySpecification<TEntity> Many<TEntity>([NotNull] Expression<Func<TEntity, object>> order, bool isDescending = false)
                where TEntity : IEntity
        {
            var spec = new GetManySpecification<TEntity>();

            spec.AddOrder(order, isDescending);

            return spec;
        }

        public static GetManySpecification<TEntity> Many<TEntity>([NotNull] Expression<Func<TEntity, bool>> filter, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending = false)
                where TEntity : IEntity
        {
            var spec = new GetManySpecification<TEntity>();

            spec.AddFilter(filter);
            spec.AddOrder(order, isDescending);

            return spec;
        }

        public static GetOneSpecification<TEntity> One<TEntity>(Expression<Func<TEntity, bool>> filter = null)
                where TEntity : IEntity
        {
            var spec = new GetOneSpecification<TEntity>();

            if (filter != null)
                spec.AddFilter(filter);

            return spec;
        }

        public static GetAllSpecification<TEntity> All<TEntity>(Expression<Func<TEntity, object>> order = null, bool isDescending = false)
                where TEntity : IEntity
        {
            var spec = new GetAllSpecification<TEntity>();

            if (order != null)
                spec.AddOrder(order, isDescending);

            return spec;
        }

        public static GetPageSpecification<TEntity> Paged<TEntity>(int pageNumber, int pageSize,[NotNull] Expression<Func<TEntity, bool>> filter)
                where TEntity : IEntity
        {
            var spec = new GetPageSpecification<TEntity>(pageSize, pageNumber);

            spec.AddFilter(filter);

            return spec;
        }

        public static GetPageSpecification<TEntity> Paged<TEntity>(int pageNumber, int pageSize, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending = false)
                where TEntity : IEntity
        {
            var spec = new GetPageSpecification<TEntity>(pageSize, pageNumber);

            spec.AddOrder(order, isDescending);

            return spec;
        }

        public static GetPageSpecification<TEntity> Paged<TEntity>(int pageNumber, int pageSize, [NotNull] Expression<Func<TEntity, bool>> filter, [NotNull] Expression<Func<TEntity, object>> order, bool isDescending = false)
                where TEntity : IEntity
        {
            var spec = new GetPageSpecification<TEntity>(pageSize, pageNumber);

            spec.AddFilter(filter);
            spec.AddOrder(order, isDescending);

            return spec;
        }
    }
}