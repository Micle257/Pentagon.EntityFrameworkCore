// -----------------------------------------------------------------------
//  <copyright file="DatabaseChangeCompareResult.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System.Collections.Generic;

    public class DatabaseChangeCompareResult<TEntity>
    {
        public DataChange<TEntity> Client { get; set; }

        public DataChange<TEntity> Server { get; set; }

        public IReadOnlyList<ConcurrencyConflictPair> Conflicts { get; set; }
    }
}