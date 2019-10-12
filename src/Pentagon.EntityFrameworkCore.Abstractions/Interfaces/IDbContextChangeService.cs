// -----------------------------------------------------------------------
//  <copyright file="IDbContextDeleteService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces
{
    /// <summary> Represents a service for delete behavior of database context. </summary>
    public interface IDbContextChangeService
    {
        /// <summary>
        /// Applies the on-save update and insert logic.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="useTimestampFromEntity">If set to <c>true</c> use timestamp from entity data; otherwise, or if the data is unspecified, from current time.</param>
        void ApplyUpdate(IApplicationContext appContext, bool useTimestampFromEntity = false);


        /// <summary>
        /// Applies the on-save concurrency stamp logic.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        void ApplyConcurrency(IApplicationContext appContext);

        /// <summary>
        /// Applies the on-save delete logic.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="useTimestampFromEntity">If set to <c>true</c> use timestamp from entity data; otherwise, or if the data is unspecified, from current time.</param>
        void ApplyDelete(IApplicationContext appContext, bool useTimestampFromEntity = false);
    }
}