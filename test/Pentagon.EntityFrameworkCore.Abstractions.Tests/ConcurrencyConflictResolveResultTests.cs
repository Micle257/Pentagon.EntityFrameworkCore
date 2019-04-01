// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictResolveResultTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Tests
{
    using System.Collections.Generic;
    using Xunit;

    public class ConcurrencyConflictResolveResultTests
    {
        [Fact]
        public void HasConflicts_ConflictsAreNull_ReturnsFalse()
        {
            var value = new ConcurrencyConflictResolveResult
                        {
                                ConflictedEntities = null
                        };

            Assert.False(value.HasConflicts);
        }

        [Fact]
        public void HasConflicts_ConflictsAreEmpty_ReturnsFalse()
        {
            var value = new ConcurrencyConflictResolveResult
                        {
                                ConflictedEntities = new List<ConcurrencyConflictPair>()
                        };

            Assert.False(value.HasConflicts);
        }

        [Fact]
        public void HasConflicts_ConflictsAreNotEmpty_ReturnsTrue()
        {
            var value = new ConcurrencyConflictResolveResult
                        {
                                ConflictedEntities = new List<ConcurrencyConflictPair>
                                                     {
                                                             new ConcurrencyConflictPair
                                                             {
                                                             }
                                                     }
                        };

            Assert.True(value.HasConflicts);
        }
    }
}