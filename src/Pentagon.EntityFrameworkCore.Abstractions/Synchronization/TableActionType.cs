// -----------------------------------------------------------------------
//  <copyright file="TableActionType.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Synchonization
{
    /// <summary> Represents a Sql table action. </summary>
    public enum TableActionType
    {
        /// <summary> The unspecified, default value. </summary>
        Unspecified,

        /// <summary> The insert command. </summary>
        Insert,

        /// <summary> The delete command. </summary>
        Delete,

        /// <summary> The update command. </summary>
        Update,

        /// <summary> The current auto increment skip command. </summary>
        Skip
    }
}