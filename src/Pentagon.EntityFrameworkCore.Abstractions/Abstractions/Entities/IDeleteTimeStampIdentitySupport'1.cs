// -----------------------------------------------------------------------
//  <copyright file="IDeleteTimeStampIdentitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IDeleteTimeStampIdentitySupport<TUserId> : IDeleteTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that deleted this entity. </summary>
        /// <value> The nullable <see cref="TUserId" />. </value>
        [Column(Order = OrderConstants.DeletedUserId)]
        new TUserId DeletedUserId { get; set; }
    }
}
