// -----------------------------------------------------------------------
//  <copyright file="EntityStateType.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    /// <summary> Represents a state of an entity on the context. </summary>
    public enum EntityStateType
    {
        /// <summary> The unspecified, default value. </summary>
        Unspecified,

        /// <summary> The entity is marked as added. </summary>
        Added,

        /// <summary> The entity is marked as deleted. </summary>
        Deleted,

        /// <summary> The entity is marked as modified. </summary>
        Modified
    }
}