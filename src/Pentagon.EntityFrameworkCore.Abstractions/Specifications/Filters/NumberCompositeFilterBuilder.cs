// -----------------------------------------------------------------------
//  <copyright file="NumberCompositeFilterBuilder.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications.Filters
{
    using System;
    using System.Linq;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class NumberCompositeFilterBuilder<TEntity> : FilterBuilder<TEntity>, INumberCompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        public NumberCompositeFilterBuilder(FilterBuilder<TEntity> parent)
        {
            TextFilters = parent.TextFilters;
            NumberFilters = parent.NumberFilters;
            Filters = parent.Filters;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value)
        {
            var lastTextFilter = NumberFilters.LastOrDefault();

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = filter;
            lastTextFilter.SecondValue = value;

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter) => AddSubFilter(operation, filter, null);
    }
}