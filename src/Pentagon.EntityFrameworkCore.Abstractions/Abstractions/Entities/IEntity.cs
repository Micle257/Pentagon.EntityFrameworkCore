// -----------------------------------------------------------------------
//  <copyright file="IEntity.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary> Represents an entity with typed boxed id. </summary>
    public interface IEntity
    {
        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The boxed value. </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        object Id { get; set; }
    }
}