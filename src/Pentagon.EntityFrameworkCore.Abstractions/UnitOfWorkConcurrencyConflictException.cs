// -----------------------------------------------------------------------
//  <copyright file="UnitOfWorkConcurrencyConflictException.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class UnitOfWorkConcurrencyConflictException : Exception
    {
        public UnitOfWorkConcurrencyConflictException() : this(message: "Concurrency error occured when updating to the database.") { }
        public UnitOfWorkConcurrencyConflictException(string message) : base(message) { }
        public UnitOfWorkConcurrencyConflictException(string message, Exception inner) : base(message, inner) { }

        protected UnitOfWorkConcurrencyConflictException(
                SerializationInfo info,
                StreamingContext context) : base(info, context) { }

        public IEnumerable<ConcurrencyConflictPair> Conflicts { get; set; }
    }
}