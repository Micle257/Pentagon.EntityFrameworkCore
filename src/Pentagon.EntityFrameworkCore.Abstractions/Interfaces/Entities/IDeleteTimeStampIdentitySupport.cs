// -----------------------------------------------------------------------
//  <copyright file="IDeleteTimeStampIdentitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    public interface IDeleteTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that deleted this entity. </summary>
        object DeletedUserId { get; set; }
    }
}