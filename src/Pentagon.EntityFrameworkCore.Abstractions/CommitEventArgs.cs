// -----------------------------------------------------------------------
//  <copyright file="CommitEventArgs.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;

    /// <summary> Provides db context commit data for handler method. </summary>
    public class CommitEventArgs : EventArgs
    {
        /// <summary> Initializes a new instance of the <see cref="CommitEventArgs" /> class. </summary>
        /// <param name="entries"> The entries. </param>
        public CommitEventArgs(params Entry[] entries)
        {
            Entries = entries;
        }

        /// <summary> Gets the entries. </summary>
        /// <value> The <see cref="IEnumerable{T}" />. </value>
        public IEnumerable<Entry> Entries { get; set; }
    }
}