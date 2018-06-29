// -----------------------------------------------------------------------
//  <copyright file="IConcurrencyStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary> Represents an entity, that supports marking by concurrency stamp. </summary>
    public interface IConcurrencyStampSupport
    {
        /// <summary> Gets or sets the concurrency stamp used as row version. </summary>
        /// <value> The <see cref="Guid" />. </value>
        [Required]
        Guid ConcurrencyStamp { get; set; }
    }
}