// -----------------------------------------------------------------------
//  <copyright file="IDeletedFlagSupport.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports deleted flag. </summary>
    public interface IDeletedFlagSupport
    {
        /// <summary> Gets or sets a value indicating whether this entity is marked as deleted when softly deleting entity, so the data of the entity aren't available. </summary>
        /// <value> <c> true </c> if this instance is deleted; otherwise, <c> false </c>. </value>
        [Column(Order = OrderConstants.DeletedFlag)]
        bool DeletedFlag { get; set; }
    }
}