// -----------------------------------------------------------------------
//  <copyright file="Entity.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    /// <summary> Represents an entity with numeric key. </summary>
    public abstract class Entity : Entity<int> { }
}