// -----------------------------------------------------------------------
//  <copyright file="ICreateStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity, that supports marking by create stamp. </summary>
    public interface ICreateStampSupport
    {
        /// <summary> Gets or sets the unique identifier of the row. </summary>
        /// <value> The <see cref="Guid" />. </value>
        [Column(Order = OrderConstants.Uuid)]
        Guid Uuid { get; set; }
    }
}