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
        protected List<Expression<Func<TEntity, bool>>> _filters = new List<Expression<Func<TEntity, bool>>>();

        [NotNull]
        protected List<ICompositeFilter> _compositeFilters = new List<ICompositeFilter>();

        [NotNull]
        protected List<Expression<Func<TEntity, bool>>> _valueFilters = new List<Expression<Func<TEntity, bool>>>();

        public IReadOnlyList<Expression<Func<TEntity, bool>>> Filters => _filters.AsReadOnly();

        public IReadOnlyList<ICompositeFilter> CompositeFilters => _compositeFilters.AsReadOnly();

        public IReadOnlyList<Expression<Func<TEntity, bool>>> ValueFilters => _valueFilters.AsReadOnly();

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

            _valueFilters.Add(expression);

            return this;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, bool>> condition)
        {
            var id = Guid.NewGuid();

            _compositeFilters.Add(new CompositeFilter<TEntity>
                                 {
                                         Id = id,
                                         FirstCondition = condition
                                 });

            return new CompositeFilterBuilder<TEntity>(this, id);
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity, TProperty> AddCompositeFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Expression<Func<TProperty, bool>> condition)
        {
            var id = Guid.NewGuid();

            _compositeFilters.Add(new CompositeFilter<TEntity, TProperty>
                                 {
                                         Id             = id,
                                         FirstCondition = condition,
                                         Property       = propertySelector
                                 });

            return new CompositeFilterBuilder<TEntity, TProperty>(this, id);
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition)
        {
            _filters.Add(condition);

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
                                                var predicate = FilterExpressionHelper.GetFilter<TEntity>(textFilter);

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
                                                var predicate = FilterExpressionHelper.GetFilter<TEntity>(textFilter);

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

            _filters.Add(predicate);

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