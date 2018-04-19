namespace Pentagon.EntityFrameworkCore {
    using System;
    using System.Collections.Generic;

    /// <summary> Provides db context commit data for handler method. </summary>
    public class ManagerCommitEventArgs : EventArgs
    {
        /// <summary> Initializes a new instance of the <see cref="CommitEventArgs" /> class. </summary>
        /// <param name="entries"> The entries. </param>
        public ManagerCommitEventArgs(Type contextType,Type entityType,params Entry[] entries)
        {
            ContextType = contextType;
            EntityType = entityType;
            Entries = entries;
        }

        public Type ContextType { get; }
        public Type EntityType { get; }

        /// <summary> Gets the entries. </summary>
        /// <value> The <see cref="IEnumerable{T}" />. </value>
        public IEnumerable<Entry> Entries { get; set; }
    }
}