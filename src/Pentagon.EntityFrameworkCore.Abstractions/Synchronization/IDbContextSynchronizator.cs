// -----------------------------------------------------------------------
//  <copyright file="IDbContextSynchronizator.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions.Entities;

    /// <summary> Represent a remote-local database synchronization service. </summary>
    public interface IDbContextSynchronizator
    {
        /// <summary> Synchronizes the local table and the remote table. </summary>
        /// <typeparam name="T"> The type of the entity. </typeparam>
        /// <param name="selector"> The selector. </param>
        /// <returns> <c> true </c> if synchronization succeed; otherwise <c> false </c>. </returns>
        Task<bool> SynchronizeAsync<T>(Expression<Func<T, bool>> selector = null)
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdatedTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new();
    }
}