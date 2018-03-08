// -----------------------------------------------------------------------
//  <copyright file="Entity'1.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework
{
    using Abstractions;
    using Abstractions.Entities;

    /// <summary> Represents an entity with typed identifier. </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        /// <inheritdoc />
        public TKey Id { get; set; }

        /// <inheritdoc />
        object IEntity.Id
        {
            get => Id;
            set => Id = (TKey) value;
        }
    }
}