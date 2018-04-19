// -----------------------------------------------------------------------
//  <copyright file="ITimeStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an entity, that supports create and update time stamps.
    /// </summary>
    public interface ITimeStampSupport
    {
        /// <summary> Gets or sets the creation time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        DateTimeOffset CreatedAt { get; set; }

        /// <summary> Gets or sets the last updated time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        DateTimeOffset? LastUpdatedAt { get; set; }
    }

    public interface ITimeStampIdentitySupport 
    {
        /// <summary> Gets or sets the user that created this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        [Required]
        object CreatedBy { get; set; }

        /// <summary> Gets or sets the user that last updated this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        object UpdatedBy { get; set; }
    }
}