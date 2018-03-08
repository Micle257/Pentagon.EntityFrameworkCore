// -----------------------------------------------------------------------
//  <copyright file="GuidEntity.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    using System;

    /// <summary> Represents an entity with GUID key. </summary>
    public abstract class GuidEntity : Entity<Guid> { }
}