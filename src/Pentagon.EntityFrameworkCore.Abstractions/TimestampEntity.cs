// -----------------------------------------------------------------------
//  <copyright file="SyncEntity.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using Abstractions.Entities;

    /// <summary> Represents an typed entity, with support for synchronization. </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    public abstract class TimestampEntity<TKey> : Entity<TKey>, ICreateStampSupport, IDeletedFlagSupport, ICreatedTimeStampSupport, IUpdatedTimeStampSupport, IDeleteTimeStampSupport
    {
        /// <inheritdoc />
        public Guid Uuid { get; set; }

        /// <inheritdoc />
        public bool IsDeletedFlag { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? DeletedAt { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}