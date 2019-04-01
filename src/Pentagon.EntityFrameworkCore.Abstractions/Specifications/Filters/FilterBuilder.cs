// -----------------------------------------------------------------------
//  <copyright file="FilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using Helpers;

    public class FilterBuilder<TEntity> : IFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        internal List<CompositeFilter<TEntity, TextFilter, string>> TextFilters = new List<CompositeFilter<TEntity, TextFilter, string>>();

        internal List<CompositeFilter<TEntity, NumberFilter, object>> NumberFilters = new List<CompositeFilter<TEntity, NumberFilter, object>>();

        internal List<Expression<Func<TEntity, bool>>> Filters = new List<Expression<Func<TEntity, bool>>>();

        internal List<Expression<Func<TEntity, bool>>> ValueFilters = new List<Expression<Func<TEntity, bool>>>();

        /// <inheritdoc />
        public FilterLogicOperation ValueFilterConcatOperation { get; set; } = FilterLogicOperation.Or;

        public bool HasAnyFilter => TextFilters.Any() || NumberFilters.Any() || Filters.Any() || ValueFilters.Any();

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values)
        {
            var expression = ValueFilterExpressionHelper.GetFilter(propertySelector, values);

            ValueFilters.Add(expression);

            return this;
        }

        /// <inheritdoc />
        public ITextCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value)
        {
            TextFilters.Add(new CompositeFilter<TEntity, TextFilter, string>
                            {
                                    Property = propertySelector,
                                    FirstCondition = filter,
                                    FirstValue = value
                            });

            return new TextCompositeFilterBuilder<TEntity>(this);
        }

        /// <inheritdoc />
        public INumberCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, NumberFilter filter, object value)
        {
            NumberFilters.Add(new CompositeFilter<TEntity, NumberFilter, object>
                              {
                                      Property = propertySelector,
                                      FirstCondition = filter,
                                      FirstValue = value
                              });

            return new NumberCompositeFilterBuilder<TEntity>(this);
        }

        /// <inheritdoc />
        public INumberCompositeFilterBuilder<TEntity> AddCompositeFilter(Expression<Func<TEntity, object>> propertySelector, NumberFilter filter) => AddCompositeFilter(propertySelector, filter, null);

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
                resultPredicate.And(b =>
                                    {
                                        foreach (var filter in Filters)
                                            b.And(filter);
                                    });
            }

            if (TextFilters.Count > 0)
            {
                resultPredicate.And(b =>
                                    {
                                        foreach (var textFilter in TextFilters)
                                        {
                                            var predicate = FilterExpressionHelper.GetFilter(textFilter);

                                            b.And(predicate);
                                        }
                                    });
            }

            if (NumberFilters.Count > 0)
            {
                resultPredicate.And(b =>
                                    {
                                        foreach (var numberFilter in NumberFilters)
                                        {
                                            var predicate = FilterExpressionHelper.GetFilter(numberFilter);

                                            b.And(predicate);
                                        }
                                    });
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