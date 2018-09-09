// -----------------------------------------------------------------------
//  <copyright file="EntityPairType.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
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