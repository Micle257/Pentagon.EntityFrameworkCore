namespace Pentagon.EntityFrameworkCore.Repositories {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;

    public class DatabaseCommitManager : IDatabaseCommitManager
    {
        public event EventHandler<ManagerCommitEventArgs> Commiting;

        public void RaiseCommit(Type contextType, Type entityType,IEnumerable<Entry> entries)
        {
            Commiting?.Invoke(this, new ManagerCommitEventArgs(contextType,entityType,entries.ToArray()));
        }
    }
}