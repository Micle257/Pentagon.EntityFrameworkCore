// -----------------------------------------------------------------------
//  <copyright file="DatabaseCommitManager.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;

    public class DatabaseCommitManager : IDatabaseCommitManager
    {
        /// <inheritdoc />
        public event EventHandler<ManagerCommitEventArgs> Commiting;

        /// <inheritdoc />
        public event EventHandler<ManagerCommitEventArgs> Commited;

        public void RaiseCommit(Type contextType, Type entityType, IEnumerable<Entry> entries)
        {
            Commiting?.Invoke(this, new ManagerCommitEventArgs(contextType, entityType, entries.ToArray()));
        }
    }
}