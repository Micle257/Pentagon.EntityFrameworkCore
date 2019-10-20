namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using Entities;

    public interface IStore<TEntity> : IRetriever<TEntity>,
                                       IDeleteStore<TEntity>,
                                       IInsertStore<TEntity>,
                                       IUpdateStore<TEntity>
            where TEntity : IEntity
    {
    }
}