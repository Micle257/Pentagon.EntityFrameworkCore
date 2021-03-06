// -----------------------------------------------------------------------
//  <copyright file="IUpdatedTimeStampSupport.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports pdate time stamps. </summary>
    public interface IUpdateTimeStampSupport
    {
        /// <summary> Gets or sets the last updated time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        [Column(Order = OrderConstants.UpdatedAt)]
        DateTimeOffset? UpdatedAt { get; set; }
    }
}