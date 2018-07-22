// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictPair.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    public class ConcurrencyConflictPair
    {
        public ConcurrencyConflictEntity Posted { get; set; }

        public ConcurrencyConflictEntity FromDatabase { get; set; }
    }
}