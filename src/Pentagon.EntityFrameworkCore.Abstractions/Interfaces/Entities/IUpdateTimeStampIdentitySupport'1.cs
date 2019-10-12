// -----------------------------------------------------------------------
//  <copyright file="IUpdateTimeStampIdentitySupport'1.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IUpdateTimeStampIdentitySupport<TUserId> : IUpdateTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that last updated this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        [Column(Order = OrderConstants.UpdatedUserId)]
        TUserId UpdatedUserId { get; set; }
    }
}