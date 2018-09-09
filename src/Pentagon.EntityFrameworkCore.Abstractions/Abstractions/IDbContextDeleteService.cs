// -----------------------------------------------------------------------
//  <copyright file="IDbContextDeleteService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;

    /// <summary> Represents a service for delete behavior of database context. </summary>
    public interface IDbContextDeleteService
    {
        /// <summary>
        /// Applies the on-save delete logic.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="useTimestampFromEntity">If set to <c>true</c> use timestamp from entity data; otherwise, or if the data is unspecified, from current time.</param>
        void Apply(IUnitOfWork unitOfWork, bool useTimestampFromEntity = false);
    }
}