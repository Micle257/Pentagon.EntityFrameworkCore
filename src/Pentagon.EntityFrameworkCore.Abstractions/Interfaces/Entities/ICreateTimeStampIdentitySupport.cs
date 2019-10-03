// -----------------------------------------------------------------------
//  <copyright file="ITimeStampSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    public interface ICreateTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that created this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        object CreatedUserId { get; set; }
    }
}