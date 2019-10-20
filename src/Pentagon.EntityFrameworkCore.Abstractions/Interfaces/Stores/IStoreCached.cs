namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using System.Threading.Tasks;
    using Entities;

    public interface IStoreCached<TEntity> : IStore<TEntity>
            where TEntity : IEntity
    {
        /// <summary>
        /// Forces the reload of the cached store.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task ReloadAsync();
    }
}