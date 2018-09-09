// -----------------------------------------------------------------------
//  <copyright file="IDatabaseCommitManager.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using System.Collections.Generic;

    public interface IDatabaseCommitManager
    {
        event EventHandler<ManagerCommitEventArgs> Commiting;

        event EventHandler<ManagerCommitEventArgs> Commited;

        void RaiseCommiting(Type contextType, Type entityType, IEnumerable<Entry> entries);

        void RaiseCommited(Type contextType, IEnumerable<Entry> entries);
    }
}