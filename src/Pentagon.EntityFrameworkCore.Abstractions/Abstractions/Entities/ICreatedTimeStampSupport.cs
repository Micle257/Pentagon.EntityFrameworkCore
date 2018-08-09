// -----------------------------------------------------------------------
//  <copyright file="ICreatedTimeStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports create time stamps. </summary>
    public interface ICreatedTimeStampSupport
    {
        /// <summary> Gets or sets the creation time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        [Column(Order = 102)]
        DateTimeOffset CreatedAt { get; set; }
    }
}