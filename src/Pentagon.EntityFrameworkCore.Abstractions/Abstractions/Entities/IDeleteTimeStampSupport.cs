// -----------------------------------------------------------------------
//  <copyright file="IDeleteTimeStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports delete time stamp. </summary>
    public interface IDeleteTimeStampSupport
    {
        /// <summary> Gets or sets the deleted time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        [Column(Order = 106)]
        DateTimeOffset? DeletedAt { get; set; }
    }
}