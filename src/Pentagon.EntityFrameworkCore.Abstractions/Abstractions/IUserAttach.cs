// -----------------------------------------------------------------------
//  <copyright file="IUserAttach.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    public interface IDataUserProvider
    {
        object UserId { get; set; }
    }
}