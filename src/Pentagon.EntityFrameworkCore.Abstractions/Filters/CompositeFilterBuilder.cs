namespace Pentagon.EntityFrameworkCore.Specifications.Filters {
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class CompositeFilterBuilder<TEntity> : FilterBuilder<TEntity>, ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        protected Guid ParentFilterId { get; }

        public CompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId)
        {
            ParentFilterId = filterId;
            TextFilters = parent.TextFilters;
            NumberFilters = parent.NumberFilters;
            Filters = parent.Filters;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = condition;

            return this;
        }

        /// <inheritdoc />
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null)
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
        public ICompositeFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null)
        {
            var lastTextFilter = TextFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), "Text filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = filter;
            lastTextFilter.SecondValue = value;

            return this;
        }
    }
}