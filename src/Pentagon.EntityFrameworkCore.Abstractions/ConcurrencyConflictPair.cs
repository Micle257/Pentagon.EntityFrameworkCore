// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictPair.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    public class ConcurrencyConflictPair
    {
        public ConcurrencyConflictEntity Local { get; set; }

        public ConcurrencyConflictEntity Remote { get; set; }
    }
}