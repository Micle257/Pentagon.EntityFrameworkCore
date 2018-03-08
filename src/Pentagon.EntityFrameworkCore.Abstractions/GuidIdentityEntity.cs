// -----------------------------------------------------------------------
//  <copyright file="GuidIdentityEntity.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    using System;

    /// <summary> Represents an identity entity with GUID key. </summary>
    /// <typeparam name="TUserKey"> The type of the user key. </typeparam>
    public abstract class TimestampIdentityGuidEntity<TUserKey> : TimestampIdentityEntity<Guid, TUserKey>
        where TUserKey : struct { }
}