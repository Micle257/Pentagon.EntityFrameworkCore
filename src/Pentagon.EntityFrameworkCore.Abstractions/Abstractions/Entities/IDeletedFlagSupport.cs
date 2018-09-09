// -----------------------------------------------------------------------
//  <copyright file="IDeletedFlagSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports deleted flag. </summary>
    public interface IDeletedFlagSupport
    {
        /// <summary> Gets or sets a value indicating whether this entity is marked as deleted when softly deleting entity, so the data of the entity aren't available. </summary>
        /// <value> <c> true </c> if this instance is deleted; otherwise, <c> false </c>. </value>
        [Column(Order = 108)]
        bool IsDeletedFlag { get; set; }
    }
}