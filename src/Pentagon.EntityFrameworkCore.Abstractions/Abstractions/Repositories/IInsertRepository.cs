// -----------------------------------------------------------------------
//  <copyright file="IInsertRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories
{
    using Entities;
    using JetBrains.Annotations;

    public interface IInsertRepository<in TEntity>
            where TEntity : IEntity
    {
        /// <summary> Adds the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        void Insert([NotNull] TEntity entity);

        /// <summary> Adds the specified entity. </summary>
        /// <typeparam name="TUserId"> The type of the user identifier. </typeparam>
        /// <param name="entity"> The entity. </param>
        /// <param name="userId"> The user identifier. </param>
        void Insert<TUserId>([NotNull] TEntity entity, [NotNull] TUserId userId);

        /// <summary> Adds the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        void InsertMany([NotNull] params TEntity[] entities);
    }
}