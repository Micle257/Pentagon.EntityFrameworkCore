// -----------------------------------------------------------------------
//  <copyright file="IDatabaseCommitManager.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using System.Collections.Generic;

    public interface IDatabaseCommitManager
    {
        event EventHandler<ManagerCommitEventArgs> Commiting;
        void RaiseCommit(Type contextType, Type entityType, IEnumerable<Entry> entries);
    }
}