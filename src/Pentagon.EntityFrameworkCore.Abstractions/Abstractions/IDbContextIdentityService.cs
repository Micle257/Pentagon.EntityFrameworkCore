// -----------------------------------------------------------------------
//  <copyright file="IDbContextIdentityService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Collections.Generic;
    using Entities;

    /// <summary> Represents a service for delete behavior of database context. </summary>
    public interface IDbContextIdentityService
    {
        /// <summary>
        /// Applies the on-save identity logic.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="userId">The user identifier that made the commit.</param>
        void Apply(IApplicationContext appContext, object userId);
    }
}