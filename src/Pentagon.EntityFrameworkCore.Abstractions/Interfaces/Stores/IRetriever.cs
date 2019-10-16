// -----------------------------------------------------------------------
//  <copyright file="IRetriever.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using Specifications;

    public interface IRetriever<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the entity by id. </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns> An awaitable <see cref="Task{TEntity}" />. </returns>
        Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        /// <summary> Counts rows in repository. </summary>
        /// <returns> A <see cref="Task" /> that represents the asynchronous operation. Result of the operation is number of rows in repository. </returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>;

        Task<IReadOnlyList<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                      TSpecification specification,
                                                                                      CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>;

        Task<IReadOnlyList<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                       TSpecification specification,
                                                                                       CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>;

        Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>;
    }
}