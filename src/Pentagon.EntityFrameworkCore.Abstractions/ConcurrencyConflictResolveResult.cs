// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictResolveResult.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Collections.Generic;

    public class ConcurrencyConflictResolveResult
    {
        public bool HasConflicts => ConflictedEntities?.Count != 0;

        public List<ConcurrencyConflictPair> ConflictedEntities { get; set; }
    }
}