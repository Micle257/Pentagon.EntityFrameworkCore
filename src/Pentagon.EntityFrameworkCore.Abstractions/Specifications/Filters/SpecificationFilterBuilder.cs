namespace Pentagon.EntityFrameworkCore.Specifications {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

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
        public IReadOnlyList<Expression<Func<TEntity, bool>>> BuildFilters()
        {
            var result = new List<Expression<Func<TEntity, bool>>>(Filters);

            foreach (var textFilter in TextFilters)
            {
                var filterType = GetTextFilterType(textFilter);

                if (filterType == 0)
                    continue;

                switch (filterType)
                {
                    case FilterCompositionType.Single:
                        result.Add(TextFilterExpressionHelper.GetFilter(textFilter.Property, textFilter.FirstCondition, textFilter.FirstValue));
                        break;
                    case FilterCompositionType.Double:
                        result.Add(TextFilterExpressionHelper.GetDoubleFilter(textFilter.Property, textFilter.FirstCondition, textFilter.FirstValue, textFilter.Operation, textFilter.SecondCondition, textFilter.SecondValue));
                        break;
                }
            }

            foreach (var numberFilter in NumberFilters)
            {
                var filterType = GetNumberFilterType(numberFilter);

                if (filterType == 0)
                    continue;

                switch (filterType)
                {
                    case FilterCompositionType.Single:
                        result.Add(NumberFilterExpressionHelper.GetFilter(numberFilter.Property, numberFilter.FirstCondition, numberFilter.FirstValue));
                        break;

                    case FilterCompositionType.Double:
                        result.Add(NumberFilterExpressionHelper.GetDoubleFilter(numberFilter.Property, numberFilter.FirstCondition, numberFilter.FirstValue, numberFilter.Operation, numberFilter.SecondCondition, numberFilter.SecondValue));
                        break;
                }
            }

            return result;
        }

        FilterCompositionType GetTextFilterType(CompositeFilter<TEntity, TextFilter, string> textFilter)
        {
            var valid = textFilter.Property != null && textFilter.FirstCondition != 0;

            if (!valid)
                return 0;

            return textFilter.Operation != 0 && textFilter.SecondCondition != 0 ? FilterCompositionType.Double : FilterCompositionType.Single;
        }

        FilterCompositionType GetNumberFilterType(CompositeFilter<TEntity, NumberFilter, object> numberFilter)
        {
            var valid = numberFilter.Property != null && numberFilter.FirstCondition != 0;

            if (!valid)
                return 0;

            return numberFilter.Operation != 0 && numberFilter.SecondCondition != 0 ? FilterCompositionType.Double : FilterCompositionType.Single;
        }
    }
}