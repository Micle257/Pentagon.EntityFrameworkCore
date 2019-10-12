// -----------------------------------------------------------------------
//  <copyright file="IDatabaseChangeManager.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.Entities;

    public interface IDatabaseChangeManager
    {
        DatabaseChangeCompareResult<T> Compare<T>(DataChange<T> client, DataChange<T> server, bool autoResolve)
                where T : class, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, new();

        DateTimeOffset? GetLastActivity<T>(IEnumerable<T> data)
                where T : class, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, new();

        Task<DataChange<T>> GetChange<T>(DateTimeOffset? lastActivityAt, IEnumerable<T> data)
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, new();
    }

    public interface IDatabaseChangeManager<TContext> : IDatabaseChangeManager
            where TContext : IApplicationContext
    {
    }
}