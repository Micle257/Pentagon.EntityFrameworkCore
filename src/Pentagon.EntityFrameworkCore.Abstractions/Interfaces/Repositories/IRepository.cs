// -----------------------------------------------------------------------
//  <copyright file="IRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Entities;
    using Specifications;
    using Stores;

    /// <summary> Provides a standard interface for repository which is data-access mechanism agnostic with typed identifier. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IRepository<TEntity> : IQueryable<TEntity>,
                                            IRetriever<TEntity>,
                                            IDeleteRepository<TEntity>,
                                            IInsertRepository<TEntity>,
                                            IUpdateRepository<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Removes all rows from repository. </summary>
        void Truncate();
    }
}