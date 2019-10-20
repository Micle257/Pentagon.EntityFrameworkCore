namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using JetBrains.Annotations;

    public interface IUpdateStore<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Updates the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        Task<ContextCommitResult<TEntity>> UpdateAsync([NotNull] TEntity entity);

        /// <summary> Updates the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        /// <returns> An awaitable <see cref="Task" />. </returns>
        Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> UpdateManyAsync([NotNull] params TEntity[] entities);
    }
}