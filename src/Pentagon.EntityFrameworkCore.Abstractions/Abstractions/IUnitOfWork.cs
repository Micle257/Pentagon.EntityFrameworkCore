// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWork.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using Entities;
    using JetBrains.Annotations;
    using Repositories;

    /// <summary> Represents a session with database provider and it is used to save data from related repositories. </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary> Gets the repository of given entity. </summary>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <typeparam name="TId"> The type of the primary key. </typeparam>
        /// <returns> A <see cref="IRepository{TEntity}" />. </returns>
        IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new();
        
         /// <summary> Gets the context. </summary>
        /// <value> The <see cref="IApplicationContext" />. </value>
        [NotNull]
        IApplicationContext Context { get; }

        /// <summary>
        /// Gets or sets a value indicating whether use time stamps from entities time stamps.
        /// </summary>
        /// <value>
        ///   <c>true</c> if time stamp source is from entity data; otherwise, <c>false</c>.
        /// </value>
        bool UseTimeSourceFromEntities { get; set; }
    }

    /// <summary> Represents an unit of work for data context and it is used to save data from related repositories. </summary>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    public interface IUnitOfWork<out TContext> : IUnitOfWork
            where TContext : IApplicationContext
    {
        /// <summary> Gets the context. </summary>
        /// <value> The <see cref="TContext" />. </value>
        [NotNull]
        TContext Context { get; }
    }
}