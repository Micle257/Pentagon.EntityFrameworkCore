namespace Pentagon.Data.EntityFramework.Abstractions.Repositories {
    using System.Threading.Tasks;
    using Entities;
    using JetBrains.Annotations;

    public interface IUpdateRepository<in TEntity>
        where TEntity : IEntity
    {
        /// <summary> Updates the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        void Update([NotNull]TEntity entity);

        /// <summary> Updates the specified entity. </summary>
        /// <typeparam name="TUserId"> The type of the user identifier. </typeparam>
        /// <param name="entity"> The entity. </param>
        /// <param name="userId"> The user identifier. </param>
        void Update<TUserId>([NotNull]TEntity entity, [NotNull] TUserId userId);

        /// <summary> Updates the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        /// <returns> An awaitable <see cref="Task" />. </returns>
        void UpdateMany([NotNull]params TEntity[] entities);
    }
}