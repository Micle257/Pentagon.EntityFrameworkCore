// -----------------------------------------------------------------------
//  <copyright file="IApplicationContext.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Abstractions
{
    using System;

    /// <summary> Represents a database application context. </summary>
    public interface IApplicationContext : IDisposable
    {
        /// <summary> Gets a value indicating whether this instance has hard delete behavior. </summary>
        /// <value> <c> true </c> if this instance has hard delete behavior; otherwise, <c> false </c>. </value>
        bool HasHardDeleteBehavior { get; }
    }
}