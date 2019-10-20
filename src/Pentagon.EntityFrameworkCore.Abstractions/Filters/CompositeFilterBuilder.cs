namespace Pentagon.EntityFrameworkCore.Filters
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Interfaces.Entities;
    using Interfaces.Filters;

    public class CompositeFilterBuilder<TEntity, TProperty> : FilterBuilder<TEntity>, ICompositeFilterBuilder<TEntity, TProperty>
            where TEntity : IEntity
    {
        protected Guid ParentFilterId { get; }

        public CompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId)
        {
            ParentFilterId   = filterId;

            _compositeFilters = parent.CompositeFilters.ToList();
           _filters          = parent.Filters.ToList();
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TProperty, bool>> condition)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            if (!(lastTextFilter is CompositeFilter<TEntity, TProperty> filter))
                throw new InvalidOperationException("Composite filter type has wrong implementation type.");

            filter.Operation       = operation;
            filter.SecondCondition = condition;

            return this;
        }
    }

    public class CompositeFilterBuilder<TEntity> : FilterBuilder<TEntity>,
                                                   ICompositeFilterBuilder<TEntity>,
                                                   IConnectedCompositeTextFilterBuilder<TEntity>,
                                                   IConnectedCompositeNumberFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        protected Guid ParentFilterId { get; }

        public CompositeFilterBuilder(FilterBuilder<TEntity> parent, Guid filterId)
        {
            ParentFilterId = filterId;

            _compositeFilters = parent.CompositeFilters.ToList();
            _filters          = parent.Filters.ToList();
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, Expression<Func<TEntity, bool>> condition)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            if (!(lastTextFilter is CompositeFilter<TEntity> filter))
                throw new InvalidOperationException("Composite filter type has wrong implementation type.");

            filter.Operation = operation;
            filter.SecondCondition = condition;

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter type, string value = null)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            if (!(lastTextFilter is CompositeFilter<TEntity> filter))
                throw new InvalidOperationException("Composite filter type has wrong implementation type.");

            if (filter?.Property == null)
                throw new ArgumentNullException(nameof(lastTextFilter), "Text filter is missing");

            filter.Operation       = operation;
            filter.SecondCondition = FilterExpressionHelper.GetTextFilterCallback(FilterExpressionHelper.GetStringPropertySelector(filter.Property), type, value);

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddOr(TextFilter filter, string value = null) => AddSubFilter(FilterLogicOperation.Or, filter, value);

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddAnd(TextFilter filter, string value = null) => AddSubFilter(FilterLogicOperation.And, filter, value);

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter type, object value = null)
        {
            var lastTextFilter = CompositeFilters.FirstOrDefault(a => a.Id == ParentFilterId);

            if (lastTextFilter == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            if (!(lastTextFilter is CompositeFilter<TEntity> filter))
                throw new InvalidOperationException("Composite filter type has wrong implementation type.");

            if (filter?.Property == null)
                throw new ArgumentNullException(nameof(lastTextFilter), message: "Number filter is missing");

            filter.Operation       = operation;
            filter.SecondCondition = FilterExpressionHelper.GetNumberFilterCallback(filter.Property, type, value);

            return this;
        }

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddOr(NumberFilter filter, object value = null) => AddSubFilter(FilterLogicOperation.Or, filter, value);

        /// <inheritdoc />
        public IFilterBuilder<TEntity> AddAnd(NumberFilter filter, object value = null) => AddSubFilter(FilterLogicOperation.And, filter, value);
    }
}