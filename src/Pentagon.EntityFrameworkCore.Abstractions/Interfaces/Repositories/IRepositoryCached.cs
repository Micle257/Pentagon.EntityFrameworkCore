namespace Pentagon.EntityFrameworkCore.Interfaces.Repositories {
    using System.Threading.Tasks;
    using Entities;

    /// <summary> Provides an extended interface for <see cref="IRepository{TEntity}"/>, this repository uses caching proxy for transient repository. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IRepositoryCached<TEntity> : IRepository<TEntity>
            where TEntity : IEntity
    {
        /// <summary>
        /// Forces the reload of the cached repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task ReloadAsync();
    }
}