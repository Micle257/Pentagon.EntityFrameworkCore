// -----------------------------------------------------------------------
//  <copyright file="DataChange.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Collections.Generic;

    public class DataChange<T>
    {
        public DateTimeOffset? LastActivityAt { get; set; }

        public List<T> Created { get; set; } = new List<T>();

        public List<T> Modified { get; set; } = new List<T>();

        public List<T> Deleted { get; set; } = new List<T>();
    }
}