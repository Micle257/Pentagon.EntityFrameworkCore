// -----------------------------------------------------------------------
//  <copyright file="IEntity'1.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    /// <summary> Represents an entity with typed id (e.g. int, string, etc.). </summary>
    /// <typeparam name="TId"> The type of the identifier. </typeparam>
    public interface IEntity<TId> : IEntity
    {
        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The <see cref="TId" />. </value>
        new TId Id { get; set; }
    }
}