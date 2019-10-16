namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using System.Threading.Tasks;
    using Entities;
    using JetBrains.Annotations;

    public interface IDeleteStore<in TEntity>
            where TEntity : IEntity
    {
        /// <summary> Removes the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        Task<ContextCommitResult> DeleteAsync([NotNull] TEntity entity);

        /// <summary> Removes the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        Task<ContextCommitResult> DeleteManyAsync([NotNull] params TEntity[] entities);
    }
}