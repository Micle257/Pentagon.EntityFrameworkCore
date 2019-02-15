// -----------------------------------------------------------------------
//  <copyright file="IRepository.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using JetBrains.Annotations;
    using Specifications;

    /// <summary> Provides a standard interface for repository which is data-access mechanism agnostic with typed identifier. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IRepository<TEntity> : IQueryable<TEntity>, IDeleteRepository<TEntity>, IInsertRepository<TEntity>, IUpdateRepository<TEntity>, IPagedRepository<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the selected entity. </summary>
        /// <param name="entitySelector"> The entity selector. </param>
        /// <param name="trackChanges"> If set to <c> true </c> the entity won't be track by change tracker. </param>
        /// <returns> An awaitable <see cref="Task{TEntity}" />. </returns>
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector);

        Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>;

        /// <summary> Gets the entity by id. </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns> An awaitable <see cref="Task{TEntity}" />. </returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary> Gets all entities in the set. </summary>
        /// <returns> A <see cref="Task" /> that represents an asynchronous operation, result is an iterator of the <see cref="TEntity" />. </returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification)
                where TSpecification : IOrderSpecification<TEntity>;

        /// <summary> Gets all selected entities from the set. </summary>
        /// <param name="entitiesSelector"> The entities selector. </param>
        /// <param name="orderSelector"> The order selector. </param>
        /// <param name="isDescending"> If set to <c> true </c> entities will be ordered from highest to lowest. </param>
        /// <returns> A <see cref="Task" /> that represents an asynchronous operation, result is an iterator of the <see cref="TEntity" />. </returns>
        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector,
                                                Expression<Func<TEntity, object>> orderSelector,
                                                bool isDescending);

        Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>;
        
        /// <summary> Counts rows in repository. </summary>
        /// <returns> A <see cref="Task" /> that represents the asynchronous operation. Result of the operation is number of rows in repository. </returns>
        Task<int> CountAsync();

        /// <summary> Removes all rows from repository. </summary>
        void Truncate();
        
        Task<TSelectEntity> GetOneAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate);

        Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>;

        Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector);

        Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IOrderSpecification<TEntity>;

        Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                     Expression<Func<TEntity, bool>> entitiesSelector,
                                                                     Expression<Func<TEntity, object>> orderSelector,
                                                                     bool isDescending);

        Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>;

        Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                   Expression<Func<TEntity, bool>> criteria,
                                                                   Expression<Func<TEntity, object>> order,
                                                                   bool isDescendingOrder,
                                                                   int pageSize,
                                                                   int pageIndex);

        Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>;
    }
}