namespace Pentagon.Data.EntityFramework.Abstractions.Repositories {
    using Entities;
    using JetBrains.Annotations;

    public interface IDeleteRepository<in TEntity>
        where TEntity : IEntity
    {
        /// <summary> Removes the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        void Delete([NotNull]TEntity entity);

        /// <summary> Removes the specified entity. </summary>
        /// <typeparam name="TUserId"> The type of the user identifier. </typeparam>
        /// <param name="entity"> The entity. </param>
        /// <param name="userId"> The user identifier. </param>
        void Delete<TUserId>([NotNull]TEntity entity, [NotNull] TUserId userId);

        /// <summary> Removes the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        void DeleteMany([NotNull]params TEntity[] entities);
    }
}