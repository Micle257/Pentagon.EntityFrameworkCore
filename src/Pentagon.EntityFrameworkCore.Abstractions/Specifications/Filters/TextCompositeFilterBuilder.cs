namespace Pentagon.EntityFrameworkCore.Specifications {
    using System;
    using System.Linq;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class TextCompositeFilterBuilder<TEntity> : SpecificationFilterBuilder<TEntity>, ITextCompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        public TextCompositeFilterBuilder(SpecificationFilterBuilder<TEntity> parent)
        {
            TextFilters = parent.TextFilters;
            NumberFilters = parent.NumberFilters;
            Filters = parent.Filters;
        }

        /// <inheritdoc />
        public ISpecificationFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null)
        {
            var lastTextFilter = TextFilters.LastOrDefault();

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), "Text filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = filter;
            lastTextFilter.SecondValue = value;

            return this;
        }
    }
}