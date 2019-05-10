namespace Pentagon.EntityFrameworkCore.Specifications.Filters {
    using System;
    using System.Linq;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class TextCompositeFilterBuilder<TEntity> : CompositeFilterBuilder<TEntity>, ITextCompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        public TextCompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId) : base(parent, filterId)
        {
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