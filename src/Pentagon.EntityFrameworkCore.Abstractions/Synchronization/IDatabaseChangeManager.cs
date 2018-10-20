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
    using Abstractions;
    using Abstractions.Entities;

    public interface IDatabaseChangeManager
    {
        DateTimeOffset? GetLastActivity<T>(IEnumerable<T> data)
                where T : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new();

        Task<DataChange<T>> GetChange<T>(DateTimeOffset? lastActivityAt, IEnumerable<T> data)
                where T : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new();
    }

    public interface IDatabaseChangeManager<TContext> : IDatabaseChangeManager
            where TContext : IApplicationContext
    {
    }
}