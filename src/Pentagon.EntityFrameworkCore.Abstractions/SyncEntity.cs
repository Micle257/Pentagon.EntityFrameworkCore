// -----------------------------------------------------------------------
//  <copyright file="SyncEntity.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using Abstractions.Entities;

    /// <summary> Represents an typed entity, with support for synchronization. </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    public abstract class SyncEntity<TKey> : Entity<TKey>, ICreateStampSupport, IConcurrencyStampSupport, IDeletedFlagSupport, ITimeStampSupport, IDeleteTimeStampSupport
    {
        /// <inheritdoc />
        public Guid ConcurrencyStamp { get; set; }

        /// <inheritdoc />
        public Guid CreateGuid { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? LastUpdatedAt { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? DeletedAt { get; set; }

        /// <inheritdoc />
        public bool IsDeletedFlag { get; set; }
    }
}