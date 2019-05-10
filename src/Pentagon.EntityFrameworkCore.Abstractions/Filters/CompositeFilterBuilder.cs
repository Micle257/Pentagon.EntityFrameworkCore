namespace Pentagon.EntityFrameworkCore.Specifications.Filters {
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class CompositeFilterBuilder<TEntity> : FilterBuilder<TEntity>, ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        public CompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId)
        {
            TextFilters = parent.TextFilters;
            NumberFilters = parent.NumberFilters;
            Filters = parent.Filters;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition, object value)
        {
            var lastTextFilter = CompositeFilters.LastOrDefault();

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = condition;
            lastTextFilter.SecondValue = value;

            return this;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition) => AddSubFilter(operation, condition, null);
    }
}