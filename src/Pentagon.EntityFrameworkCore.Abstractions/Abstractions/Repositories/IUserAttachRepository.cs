// -----------------------------------------------------------------------
//  <copyright file="IUserAttachRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Repositories
{
    public interface IUserAttachRepository
    {
        bool IsUserAttached { get; }

        object UserId { get; set; }
    }
}