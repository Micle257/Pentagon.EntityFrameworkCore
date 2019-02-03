// -----------------------------------------------------------------------
//  <copyright file="Entity'1.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Abstractions.Entities;

    /// <summary> Represents an entity with typed identifier. </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        /// <inheritdoc />
        public virtual TKey Id { get; set; }

        /// <inheritdoc />
        object IEntity.Id
        {
            get => Id;
            set => Id = (TKey) value;
        }
    }
}