// -----------------------------------------------------------------------
//  <copyright file="IDbContextUpdateService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;

    /// <summary> Represents a service for update and insert behavior of database context. </summary>
    public interface IDbContextUpdateService
    {
        /// <summary>
        /// Applies the on-save update and insert logic.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="useTimestampFromEntity">If set to <c>true</c> use timestamp from entity data; otherwise, or if the data is unspecified, from current time.</param>
        void Apply(IApplicationContext appContext, bool useTimestampFromEntity = false);
    }
}