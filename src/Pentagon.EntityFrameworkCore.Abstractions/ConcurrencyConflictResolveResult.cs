// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictResolveResult.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Collections.Generic;
    using Abstractions.Entities;

    public class ConcurrencyConflictResolveResult
    {
        public bool HasConflicts => ConflictedEntities?.Count != 0;

        public List<(IEntity Local, IEntity Database)> ConflictedEntities { get; set; }
    }
}