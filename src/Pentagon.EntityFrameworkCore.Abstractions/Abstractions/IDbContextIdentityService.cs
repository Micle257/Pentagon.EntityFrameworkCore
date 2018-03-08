// -----------------------------------------------------------------------
//  <copyright file="IDbContextIdentityService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Abstractions
{
    using System.Collections.Generic;
    using Entities;

    /// <summary> Represents a service for delete behavior of database context. </summary>
    public interface IDbContextIdentityService
    {
        /// <summary> Applies the on-save identity logic. </summary>
        /// <param name="appContext"> The application context. </param>
        /// <param name="entryMap"> The entry map. </param>
        void Apply(IApplicationContext appContext, IDictionary<IEntity, object> entryMap);
    }
}