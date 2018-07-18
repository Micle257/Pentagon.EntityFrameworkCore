// -----------------------------------------------------------------------
//  <copyright file="IDeleteRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories
{
    using Entities;
    using JetBrains.Annotations;

    public interface IDeleteRepository<in TEntity>
            where TEntity : IEntity
    {
        /// <summary> Removes the specified entity. </summary>
        /// <param name="entity"> The entity. </param>
        void Delete([NotNull] TEntity entity);
        
        /// <summary> Removes the array of entities. </summary>
        /// <param name="entities"> The array of the <see cref="TEntity" />. </param>
        void DeleteMany([NotNull] params TEntity[] entities);
    }
}