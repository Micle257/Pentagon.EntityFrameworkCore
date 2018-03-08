// -----------------------------------------------------------------------
//  <copyright file="IRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Entities;
    using Specifications;

    /// <summary> Provides a standard interface for repository which is data-access mechanism agnostic with typed identifier. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IRepository<TEntity> : IDeleteRepository<TEntity>, IInsertRepository<TEntity>, IUpdateRepository<TEntity>, IPagedRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary> Occurs when the commit begins. </summary>
        event EventHandler<CommitEventArgs> Commiting;

        /// <summary>
        /// Apply the extra query to the request pipeline.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns>The <see cref="IRepository{TEntity}"/> that called this method.</returns>
        IRepository<TEntity> Query(Func<IQueryable<TEntity>, IQueryable<TEntity>> function);

        /// <summary>
        /// Gets the selected entity.
        /// </summary>
        /// <param name="entitySelector">The entity selector.</param>
        /// <param name="trackChanges">If set to <c>true</c> the entity won't be track by change tracker.</param>
        /// <returns>
        /// An awaitable <see cref="Task{TEntity}" />.
        /// </returns>
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector, bool trackChanges = false);

        Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification)
            where TSpecification : ICriteriaSpecification<TEntity>;

        /// <summary> Gets the entity by id. </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns> An awaitable <see cref="Task{TEntity}" />. </returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary> Gets all entities in the set. </summary>
        /// <param name="asNoTracking">If set to <c>true</c> the entities won't be track by change tracker.</param>
        /// <returns> A <see cref="Task" /> that represents an asynchronous operation, result is an iterator of the <see cref="TEntity" />. </returns>
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);

        Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification)
            where TSpecification : IOrderSpecification<TEntity>;

        /// <summary> Gets all selected entities from the set. </summary>
        /// <param name="entitiesSelector"> The entities selector. </param>
        /// <param name="asNoTracking">If set to <c>true</c> the entities won't be track by change tracker.</param>
        /// <returns> An awaitable enumerable of the <see cref="TEntity" />. </returns>
        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector, bool asNoTracking = false);

        Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification)
            where TSpecification : ICriteriaSpecification<TEntity>, IOrderSpecification<TEntity>;

        /// <summary> Gets a property, for navigational property. </summary>
        /// <param name="foreignKey"> The foreign key to the navigational property. </param>
        /// <returns> A <see cref="Task" /> that represents the asynchronous operation, result is the selected property. </returns>
        Task<TProperty> GetPropertyByForeignKeyAsync<TProperty>(object foreignKey)
            where TProperty : class;

        /// <summary> Counts rows in repository. </summary>
        /// <returns> A <see cref="Task" /> that represents the asynchronous operation. Result of the operation is number of rows in repository. </returns>
        Task<int> CountAsync();

        /// <summary> Removes all rows from repository. </summary>
        void Truncate();
    }
}