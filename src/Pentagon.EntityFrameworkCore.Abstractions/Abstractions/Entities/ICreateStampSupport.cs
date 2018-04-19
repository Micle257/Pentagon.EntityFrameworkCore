// -----------------------------------------------------------------------
//  <copyright file="ICreateStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an entity, that supports marking by create stamp.
    /// </summary>
    public interface ICreateStampSupport
    {
        /// <summary> Gets or sets the unique identifier of the row. </summary>
        /// <value> The <see cref="Guid" />. </value>
        [Required]
        Guid CreateGuid { get; set; }
    }
}