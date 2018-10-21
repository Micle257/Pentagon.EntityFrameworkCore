// -----------------------------------------------------------------------
//  <copyright file="IApplicationContext.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using Entities;
    using Repositories;

    /// <summary> Represents a database application context. </summary>
    public interface IApplicationContext : IDisposable
    {
        bool UseTimeSourceFromEntities { get; set; }

        event EventHandler<ManagerCommitEventArgs> Commiting;

        /// <summary> Gets the repository of given entity. </summary>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <returns> A <see cref="IRepository{TEntity}" />. </returns>
        IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new();
    }
}