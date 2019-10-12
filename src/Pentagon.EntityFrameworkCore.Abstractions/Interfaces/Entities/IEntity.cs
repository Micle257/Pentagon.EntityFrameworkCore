// -----------------------------------------------------------------------
//  <copyright file="IEntity.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    /// <summary> Represents an entity with typed boxed id. </summary>
    public interface IEntity
    {
        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The boxed value. </value>
        object Id { get; set; }
    }
}