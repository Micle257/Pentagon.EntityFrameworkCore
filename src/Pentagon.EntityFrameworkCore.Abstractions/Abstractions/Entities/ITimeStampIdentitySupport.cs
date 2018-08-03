// -----------------------------------------------------------------------
//  <copyright file="ITimeStampIdentitySupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    using System.ComponentModel.DataAnnotations;

    public interface ITimeStampIdentitySupport<TUserId> : ITimeStampIdentitySupport
            where TUserId : struct
    {
        /// <summary> Gets or sets the user that created this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        TUserId? CreatedBy { get; set; }

        /// <summary> Gets or sets the user that last updated this entity. </summary>
        /// <value> The <see cref="object" />. </value>
        TUserId? UpdatedBy { get; set; }
    }
}
