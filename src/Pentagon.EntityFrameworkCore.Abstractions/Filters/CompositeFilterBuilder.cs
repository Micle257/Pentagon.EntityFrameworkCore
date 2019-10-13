namespace Pentagon.EntityFrameworkCore.Filters
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Interfaces.Entities;
    using Interfaces.Filters;

    public class CompositeFilterBuilder<TEntity> : FilterBuilder<TEntity>, IConnectedCompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        protected Guid ParentFilterId { get; }

        public CompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId)
        {
            ParentFilterId = filterId;
            CompositeFilters = parent.CompositeFilters;
            Filters = parent.Filters;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = condition;

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter?.Property == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = FilterExpressionHelper.GetNumberFilterCallback(lastTextFilter.Property, filter, value);

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddOr(NumberFilter filter, object value = null) => AddSubFilter(FilterLogicOperation.Or, filter, value);

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddAnd(NumberFilter filter, object value = null) => AddSubFilter(FilterLogicOperation.And, filter, value);

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter?.Property == null)
                throw new ArgumentNullException(nameof(lastTextFilter), "Text filter is missing");

            lastTextFilter.Operation = operation;
            lastTextFilter.SecondCondition = FilterExpressionHelper.GetTextFilterCallback( FilterExpressionHelper.GetStringPropertySelector(lastTextFilter.Property), filter, value);

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddOr(TextFilter filter, string value = null) => AddSubFilter(FilterLogicOperation.Or, filter, value);

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddAnd(TextFilter filter, string value = null) => AddSubFilter(FilterLogicOperation.And, filter, value);
    }
}