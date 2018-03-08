// -----------------------------------------------------------------------
//  <copyright file="IdentityEntity'2.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    using Abstractions;
    using Abstractions.Entities;

    /// <summary> Represents an identity type entity, with synchronization support. </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <typeparam name="TUserKey"> The type of the user key. </typeparam>
    public abstract class TimestampIdentityEntity<TKey, TUserKey> : SyncEntity<TKey>, ITimeStampIdentitySupport<TUserKey>, IDeleteTimeStampIdentitySupport<TUserKey>
        where TUserKey : struct
    {
        /// <inheritdoc />
        public TUserKey CreatedBy { get; set; }
        
        /// <inheritdoc />
        public TUserKey? UpdatedBy { get; set; }

        /// <inheritdoc />
        public TUserKey? DeletedBy { get; set; }

        /// <inheritdoc />
        object ITimeStampIdentitySupport.CreatedBy
        {
            get => CreatedBy;
            set => CreatedBy = (TUserKey) value;
        }

        /// <inheritdoc />
        object ITimeStampIdentitySupport.UpdatedBy
        {
            get => UpdatedBy;
            set => UpdatedBy = (TUserKey?) value;
        }

        /// <inheritdoc />
        object IDeleteTimeStampIdentitySupport.DeletedBy
        {
            get => DeletedBy;
            set => DeletedBy = (TUserKey?) value;
        }
    }
}