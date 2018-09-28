namespace Pentagon.EntityFrameworkCore.Specifications {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using Helpers;

    public class SpecificationFilterBuilder<TEntity> : ISpecificationFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        internal List<CompositeFilter<TEntity, TextFilter, string>> TextFilters = new List<CompositeFilter<TEntity, TextFilter, string>>();

        internal List<CompositeFilter<TEntity, NumberFilter, object>> NumberFilters = new List<CompositeFilter<TEntity, NumberFilter, object>>();

        internal List<Expression<Func<TEntity, bool>>> Filters = new List<Expression<Func<TEntity, bool>>>();

        /// <inheritdoc />
        public ISpecificationFilterBuilder<TEntity> AddValueFilter<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, params TProperty[] values)
        {
            var expression = ValueFilterExpressionHelper.GetFilter(propertySelector, values);

            Filters.Add(expression);

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
        public ISpecificationFilterBuilder<TEntity> AddFilter(Expression<Func<TEntity, bool>> condition)
        {
            Filters.Add(condition);

            return this;
        }

        /// <inheritdoc />
        public ISpecificationFilterBuilder<TEntity> AddFilter(Action<IPredicateBuilder<TEntity>> build)
        {
            var builder = new PredicateBuilder<TEntity>();

            build(builder);

            var predicate = builder.Build();

            Filters.Add(predicate);

            return this;
        }

        /// <inheritdoc />
        public IReadOnlyList<Expression<Func<TEntity, bool>>> BuildFilters()
        {
            var result = new List<Expression<Func<TEntity, bool>>>(Filters);

            foreach (var textFilter in TextFilters)
            {
                var predicate = FilterExpressionHelper.GetFilter(textFilter);

                result.Add(predicate);

               //var filterType = GetFilterType(textFilter);
               //
               //if (filterType == 0)
               //    continue;
               //
               //switch (filterType)
               //{
               //    case FilterCompositionType.Single:
               //        result.Add(TextFilterExpressionHelper.GetFilter(textFilter.Property, textFilter.FirstCondition.Value, textFilter.FirstValue));
               //        break;
               //    case FilterCompositionType.Double:
               //        result.Add(TextFilterExpressionHelper.GetDoubleFilter(textFilter.Property, textFilter.FirstCondition.Value, textFilter.FirstValue, textFilter.Operation, textFilter.SecondCondition.Value, textFilter.SecondValue));
               //        break;
               //}
            }

            foreach (var numberFilter in NumberFilters)
            {
                var predicate = FilterExpressionHelper.GetFilter(numberFilter);

                result.Add(predicate);
                // var filterType = GetFilterType(numberFilter);
                //
                // if (filterType == 0)
                //     continue;
                //
                // switch (filterType)
                // {
                //     case FilterCompositionType.Single:
                //         result.Add(NumberFilterExpressionHelper.GetFilter(numberFilter.Property, numberFilter.FirstCondition.Value, numberFilter.FirstValue));
                //         break;
                //
                //     case FilterCompositionType.Double:
                //         result.Add(NumberFilterExpressionHelper.GetDoubleFilter(numberFilter.Property, numberFilter.FirstCondition.Value, numberFilter.FirstValue, numberFilter.Operation, numberFilter.SecondCondition.Value, numberFilter.SecondValue));
                //         break;
                // }
            }

            return result;
        }
    }
}