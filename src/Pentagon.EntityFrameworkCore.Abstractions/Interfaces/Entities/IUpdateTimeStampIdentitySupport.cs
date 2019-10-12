// -----------------------------------------------------------------------
//  <copyright file="IUpdateTimeStampIdentitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    public interface IUpdateTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that last updated this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        object UpdatedUserId { get; set; }
    }
}