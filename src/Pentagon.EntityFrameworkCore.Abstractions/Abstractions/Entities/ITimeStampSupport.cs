// -----------------------------------------------------------------------
//  <copyright file="ITimeStampSupport.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
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