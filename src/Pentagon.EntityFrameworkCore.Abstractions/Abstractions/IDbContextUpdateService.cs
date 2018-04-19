﻿// -----------------------------------------------------------------------
//  <copyright file="IDbContextUpdateService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    /// <summary> Represents a service for update and insert behavior of database context. </summary>
    public interface IDbContextUpdateService
    {
        /// <summary> Applies the on-save update and insert logic. </summary>
        /// <param name="appContext"> The application context. </param>
        void Apply(IApplicationContext appContext);
    }
}