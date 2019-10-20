namespace Pentagon.EntityFrameworkCore.Interfaces.Stores {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;
    using JetBrains.Annotations;

    public interface IInsertStore<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Adds the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        Task<ContextCommitResult<TEntity>> InsertAsync([NotNull] TEntity entity);

        /// <summary> Adds the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> InsertManyAsync([NotNull] params TEntity[] entities);
    }
}