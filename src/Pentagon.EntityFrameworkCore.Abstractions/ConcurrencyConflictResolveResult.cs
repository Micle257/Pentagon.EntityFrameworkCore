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
        public bool CanBeDetermine { get; set; } = true;

        public bool HasConflicts =>  ConflictedEntities != null && ConflictedEntities.Count > 0;

        public List<ConcurrencyConflictPair> ConflictedEntities { get; set; } = new List<ConcurrencyConflictPair>();
    }
}