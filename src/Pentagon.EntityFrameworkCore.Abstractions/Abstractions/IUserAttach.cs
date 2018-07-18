// -----------------------------------------------------------------------
//  <copyright file="IUserAttachRepository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    public interface IUserAttach
    {
        bool IsUserAttached { get; }

        object UserId { get; set; }
    }
}