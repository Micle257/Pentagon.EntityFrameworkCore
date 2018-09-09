// -----------------------------------------------------------------------
//  <copyright file="IDeleteTimeStampIdentitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    public interface IDeleteTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that deleted this entity. </summary>
        /// <value> The nullable <see cref="TUserId" />. </value>
        object DeletedUserId { get; set; }
    }
}