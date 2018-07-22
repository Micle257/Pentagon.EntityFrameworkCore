// -----------------------------------------------------------------------
//  <copyright file="ICreatedTimeStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;

    /// <summary> Represents an entity, that supports create time stamps. </summary>
    public interface ICreatedTimeStampSupport
    {
        /// <summary> Gets or sets the creation time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        DateTimeOffset CreatedAt { get; set; }
    }
}