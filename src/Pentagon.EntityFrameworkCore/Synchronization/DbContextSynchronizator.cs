﻿// -----------------------------------------------------------------------
//  <copyright file="DbContextSynchronizator.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Interfaces.Entities;

    /// <summary> Provides logic for synchronization local database with remote database. </summary>
    public class DbContextSynchronizator : IDbContextSynchronizator
    {
        readonly ISynchronizationFactory _factory;

        public DbContextSynchronizator(ISynchronizationFactory factory)
        {
            _factory = factory;
        }

        /// <summary> Synchronizes the local table and the remote table. </summary>
        /// <typeparam name="T"> The type of the entity. </typeparam>
        /// <param name="selector"> The selector. </param>
        /// <returns> <c> true </c> if synchronization succeed; otherwise <c> false </c>. </returns>
        public async Task<bool> SynchronizeAsync<T>(Expression<Func<T, bool>> selector = null)
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
        {
            if (selector == null)
                selector = a => true;

            var session = _factory.Create<T>();

            await session.SynchronizeAsync(selector).ConfigureAwait(false);

            return true;
        }
    }
}