// -----------------------------------------------------------------------
//  <copyright file="ITimeStampIdentitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public interface ITimeStampIdentitySupport<TUserId> : ITimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that created this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        [Column(Order = 103)]
        TUserId CreatedBy { get; set; }

        /// <summary> Gets or sets the user that last updated this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        [Column(Order = 105)]
        TUserId UpdatedBy { get; set; }
    }
}
