// -----------------------------------------------------------------------
//  <copyright file="IUserIdentitySupport.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    public interface IUserIdentitySupport<TUserId, TUser>
    {
        TUserId UserId { get; set; }

        TUser User { get; set; }
    }
}