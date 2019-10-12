// -----------------------------------------------------------------------
//  <copyright file="ICreateUserEntitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface ICreatedUserEntitySupport
    {
        /// <summary> Gets or sets the user's name that created this entity (row). </summary>
        /// <value> The <see cref="string" />. </value>
        [Column(Order = OrderConstants.CreatedUser)]
        string CreatedUser { get; set; }
    }
}