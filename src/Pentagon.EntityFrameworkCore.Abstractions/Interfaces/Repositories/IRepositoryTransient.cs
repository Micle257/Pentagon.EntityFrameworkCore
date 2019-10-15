namespace Pentagon.EntityFrameworkCore.Interfaces.Repositories {
    using Entities;

    /// <summary> Provides an extended interface for <see cref="IRepository{TEntity}"/>, this repository calls EF.Core directly. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IRepositoryTransient<TEntity> : IRepository<TEntity>
            where TEntity : IEntity {

    }
}