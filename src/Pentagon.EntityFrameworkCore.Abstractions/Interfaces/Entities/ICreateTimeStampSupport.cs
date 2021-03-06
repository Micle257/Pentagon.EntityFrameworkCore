// -----------------------------------------------------------------------
//  <copyright file="ICreatedTimeStampSupport.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports create time stamps. </summary>
    public interface ICreateTimeStampSupport
    {
        /// <summary> Gets or sets the creation time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        [Column(Order = OrderConstants.CreatedAt)]
        DateTimeOffset CreatedAt { get; set; }
    }
}