// -----------------------------------------------------------------------
//  <copyright file="ICreateTimeStampIdentitySupport'1.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface ICreateTimeStampIdentitySupport<TUserId> : ICreateTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that created this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        [Column(Order = OrderConstants.CreatedUserId)]
        new TUserId CreatedUserId { get; set; }
    }
}