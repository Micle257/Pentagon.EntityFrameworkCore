// -----------------------------------------------------------------------
//  <copyright file="IdentityEntity'2.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Interfaces.Entities;

    /// <summary> Represents an identity type entity, with synchronization support. </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <typeparam name="TUserKey"> The type of the user key for identity fields. </typeparam>
    public abstract class TimestampIdentityEntity<TKey, TUserKey> : TimestampEntity<TKey>, ICreateTimeStampIdentitySupport<TUserKey>, IUpdateTimeStampIdentitySupport<TUserKey>, IDeleteTimeStampIdentitySupport<TUserKey>
    {
        /// <inheritdoc />
        public TUserKey DeletedUserId { get; set; }

        /// <inheritdoc />
        public TUserKey CreatedUserId { get; set; }

        /// <inheritdoc />
        public TUserKey UpdatedUserId { get; set; }

        /// <inheritdoc />
        object IDeleteTimeStampIdentitySupport.DeletedUserId
        {
            get => DeletedUserId;
            set => DeletedUserId = (TUserKey) value;
        }

        /// <inheritdoc />
        object ICreateTimeStampIdentitySupport.CreatedUserId
        {
            get => CreatedUserId;
            set => CreatedUserId = (TUserKey) value;
        }

        /// <inheritdoc />
        object IUpdateTimeStampIdentitySupport.UpdatedUserId
        {
            get => UpdatedUserId;
            set => UpdatedUserId = (TUserKey) value;
        }
    }
}