// -----------------------------------------------------------------------
//  <copyright file="ILocalContext.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    /// <summary> Represents a local (on client) database context. </summary>
    public interface ILocalContext : IApplicationContext { }
}