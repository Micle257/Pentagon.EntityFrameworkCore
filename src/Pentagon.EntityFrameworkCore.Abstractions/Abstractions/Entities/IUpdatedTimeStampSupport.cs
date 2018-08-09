// -----------------------------------------------------------------------
//  <copyright file="IUpdatedTimeStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports pdate time stamps. </summary>
    public interface IUpdatedTimeStampSupport
    {
        /// <summary> Gets or sets the last updated time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        [Column(Order = 104)]
        DateTimeOffset? LastUpdatedAt { get; set; }
    }
}