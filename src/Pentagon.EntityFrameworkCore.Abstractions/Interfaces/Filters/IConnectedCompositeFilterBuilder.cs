namespace Pentagon.EntityFrameworkCore.Interfaces.Filters {
    using System;
    using Entities;
    using EntityFrameworkCore.Filters;

    public interface IConnectedCompositeFilterBuilder<TEntity> : ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase);
        
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null);
    }
}