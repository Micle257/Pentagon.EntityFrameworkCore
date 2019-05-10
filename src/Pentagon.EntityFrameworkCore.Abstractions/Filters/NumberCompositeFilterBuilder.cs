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

    public class NumberCompositeFilterBuilder<TEntity> : CompositeFilterBuilder<TEntity>, INumberCompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        public NumberCompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId) : base(parent, filterId)
        {
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value)
        {
            var lastTextFilter = NumberFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = filter;
            lastTextFilter.SecondValue = value;

            return this;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter) => AddSubFilter(operation, filter, null);
    }
}