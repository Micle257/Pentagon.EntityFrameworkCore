namespace Pentagon.EntityFrameworkCore.Interfaces.Filters {
    using Entities;
    using EntityFrameworkCore.Filters;

    public interface IConnectedCompositeFilterBuilder<TEntity> : ICompositeFilterBuilder<TEntity>
            where TEntity : IEntity
    {
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, TextFilter filter, string value = null);
        
        IFilterBuilder<TEntity> AddSubFilter(FilterLogicOperation operation, NumberFilter filter, object value = null);
    }
}