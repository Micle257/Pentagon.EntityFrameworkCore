namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using Entities;

    public interface IStoreTransient<TEntity> : IStore<TEntity>
            where TEntity : IEntity
    {

    }
}