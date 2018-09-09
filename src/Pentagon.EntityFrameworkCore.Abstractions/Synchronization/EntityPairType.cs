// -----------------------------------------------------------------------
//  <copyright file="EntityPairType.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    public enum EntityPairType
    {
        Unspecified,
        LocalOnly,
        RemoteOnly,
        Both
    }
}