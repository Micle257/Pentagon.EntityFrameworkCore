// -----------------------------------------------------------------------
//  <copyright file="IdentityEntity.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    /// <summary> Represents an identity entity with numeric key. </summary>
    /// <typeparam name="TUserKey"> The type of the user key. </typeparam>
    public abstract class TimestampIdentityEntity<TUserKey> : TimestampIdentityEntity<int, TUserKey>
        where TUserKey : struct { }
}