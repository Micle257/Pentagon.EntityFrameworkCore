// -----------------------------------------------------------------------
//  <copyright file="Entry.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Interfaces.Entities;

    /// <summary> Provides information about <see cref="IEntity" /> that changes its state. </summary>
    public class Entry
    {
        /// <summary> Initializes a new instance of the <see cref="Entry" /> class. </summary>
        /// <param name="entity"> The entity. </param>
        /// <param name="state"> The state. </param>
        /// <param name="userId"> The user identifier. </param>
        public Entry(IEntity entity, EntityStateType state, object userId = null)
        {
            Entity = entity;
            State = state;
            UserId = userId;
        }

        /// <summary> Gets the state. </summary>
        /// <value> The <see cref="EntityStateType" />. </value>
        public EntityStateType State { get; }

        /// <summary> Gets the user identifier. </summary>
        /// <value> The <see cref="object" />. </value>
        public object UserId { get; }

        /// <summary> Gets the entity. </summary>
        /// <value> The <see cref="IEntity" />. </value>
        public IEntity Entity { get; }
    }
}