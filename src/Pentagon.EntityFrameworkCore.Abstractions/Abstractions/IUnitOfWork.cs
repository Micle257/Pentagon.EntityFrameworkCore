// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWork.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using System.Threading.Tasks;
    using Entities;
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

        /// <summary> Commits the changes of this unit of work. </summary>
        /// <returns> An <see cref="int" />, that represents number of entities states written to the database. </returns>
        int Commit();

        /// <summary> Commits the changes of this unit of work asynchronously. </summary>
        /// <returns> A <see cref="Task" /> that represents asynchronous operation, result is a number of entities states written to the database. </returns>
        Task<int> CommitAsync();
    }
}