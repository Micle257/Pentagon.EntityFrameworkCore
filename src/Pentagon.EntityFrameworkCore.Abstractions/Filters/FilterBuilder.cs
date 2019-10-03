// -----------------------------------------------------------------------
//  <copyright file="FilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Helpers;
    using Interfaces.Entities;
    using Interfaces.Filters;
    using JetBrains.Annotations;

    public class FilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        [NotNull]
        internal List<Expression<Func<TEntity, bool>>> Filters = new List<Expression<Func<TEntity, bool>>>();

        [NotNull]
        internal List<CompositeFilter<TEntity, object>> CompositeFilters = new List<CompositeFilter<TEntity, object>>();

        [NotNull]
        internal List<Expression<Func<TEntity, bool>>> ValueFilters = new List<Expression<Func<TEntity, bool>>>();

        /// <inheritdoc />
        public FilterLogicOperation ValueFilterConcatOperation { get; set; } = FilterLogicOperation.Or;

        /// <inheritdoc />
        public FilterLogicOperation FilterConcatOperation { get; set; } = FilterLogicOperation.And;

        /// <inheritdoc />
        public bool HasAnyFilter => CompositeFilters.Any() || Filters.Any() || ValueFilters.Any();

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values)
        {
            var expression = ValueFilterExpressionHelper.GetFilter(propertySelector, values);

            ValueFilters.Add(expression);

            return this;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, bool>> condition)
        {
            var id = Guid.NewGuid();

            CompositeFilters.Add(new CompositeFilter<TEntity, object>
                                 {
                                         Id = id,
                                         FirstCondition = condition
                                 });

            return new CompositeFilterBuilder<TEntity>(this, id);
        }

        /// <inheritdoc />
        public IConnectedCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, Expression<Func<TEntity, bool>> condition)
        {
            var id = Guid.NewGuid();

            CompositeFilters.Add(new CompositeFilter<TEntity, object>
                                 {
                                         Id = id,
                                         FirstCondition = condition,
                                         Property = propertySelector
                                 });

            return new CompositeFilterBuilder<TEntity>(this, id);
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition)
        {
            Filters.Add(condition);

            return this;
        }

        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> BuildFilter()
        {
            if (!HasAnyFilter)
            {
                return a => true;
            }

            var resultPredicate = new PredicateBuilder<TEntity>();
            
            if (Filters.Count > 0)
            {

                if (FilterConcatOperation == FilterLogicOperation.Or)
                {
                    resultPredicate.Or(b =>
                                       {
                                           foreach (var filter in Filters)
                                               b.Or(filter);
                                       });
                }
                else
                {
                    resultPredicate.And(b =>
                                        {
                                            foreach (var filter in Filters)
                                                b.And(filter);
                                        });
                }
            }

            if (CompositeFilters.Count > 0)
            {
                if (FilterConcatOperation == FilterLogicOperation.Or)
                {
                    resultPredicate.Or(b =>
                                        {
                                            foreach (var textFilter in CompositeFilters)
                                            {
                                                var predicate = FilterExpressionHelper.GetFilter(textFilter);

                                                b.Or(predicate);
                                            }
                                        });
                }
                else
                {
                    resultPredicate.And(b =>
                                        {
                                            foreach (var textFilter in CompositeFilters)
                                            {
                                                var predicate = FilterExpressionHelper.GetFilter(textFilter);

                                                b.And(predicate);
                                            }
                                        });
                }
            }

            if (ValueFilters.Count > 0)
            {
                if (ValueFilterConcatOperation == FilterLogicOperation.And)
                    resultPredicate.And(ConfigureValueFilters());
                else
                    resultPredicate.Or(ConfigureValueFilters());
            }

            return resultPredicate.Build();
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddFilter(Action<IPredicateBuilder<TEntity>> build)
        {
            var builder = new PredicateBuilder<TEntity>();

            build(builder);

            var predicate = builder.Build();

            Filters.Add(predicate);

            return this;
        }

        Action<IPredicateBuilder<TEntity>> ConfigureValueFilters()
        {
            return b =>
                   {
                       foreach (var filter in ValueFilters)
                           b.And(filter);
                   };
        }
    }
}