// -----------------------------------------------------------------------
//  <copyright file="IDbContextDeleteService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Abstractions
{
    /// <summary> Represents a service for delete behavior of database context. </summary>
    public interface IDbContextDeleteService
    {
        /// <summary> Applies the on-save delete logic. </summary>
        /// <param name="appContext"> The application context. </param>
        /// <param name="isHardDelete"> If set to <c> true </c> the deletion is hard (permanently deleted from database); otherwise the deletion is marked by IsDeleted property. </param>
        void Apply(IApplicationContext appContext, bool? isHardDelete = null);
    }
}