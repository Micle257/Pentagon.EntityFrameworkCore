// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWork.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using Entities;
    using Repositories;

    /// <summary> Represents a session with database provider and it is used to save data from related repositories. </summary>
    public interface IUnitOfWork : IDisposable, IUserAttach
    {
        /// <summary> Gets the repository of given entity. </summary>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <typeparam name="TId"> The type of the primary key. </typeparam>
        /// <returns> A <see cref="IRepository{TEntity}" />. </returns>
        IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new();
    }
}