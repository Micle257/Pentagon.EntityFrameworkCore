// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkCommitResult.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using OperationResults;

    public class ContextCommitResult : OperationResult<int>
    {
        public bool HasConcurrencyConflicts => Conflicts != null && Conflicts.Count > 0;

        public List<ConcurrencyConflictPair> Conflicts { get; set; } = new List<ConcurrencyConflictPair>();
    }

    public class ContextCommitResult<TResult> : ContextCommitResult
    {
        public TResult Result { get; set; }
    }
}