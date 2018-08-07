// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkCommitResult.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;

    public class UnitOfWorkCommitResult
    {
        public bool IsSuccessful => Exception != null && !HasConcurrencyConflicts;

        public bool HasConcurrencyConflicts => Conflicts.Count != 0;

        public Exception Exception { get; set; }

        public List<ConcurrencyConflictPair> Conflicts { get; set; }
    }
}