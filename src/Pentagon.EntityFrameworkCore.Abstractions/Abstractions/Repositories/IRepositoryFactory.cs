// -----------------------------------------------------------------------
//  <copyright file="IRepositoryFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories
{
    using Entities;
    using JetBrains.Annotations;

    /// <summary> Provides a factory for creating repositories. </summary>
    public interface IRepositoryFactory
    {
        /// <summary> Gets the repository of given type. </summary>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <param name="context"> The context. </param>
        /// <returns> A <see cref="IRepository{TEntity}" />. </returns>
        [NotNull]
        IRepository<TEntity> GetRepository<TEntity>([NotNull] IApplicationContext context)
            where TEntity : class, IEntity, new();
    }
}