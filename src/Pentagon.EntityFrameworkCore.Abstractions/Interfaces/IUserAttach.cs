// -----------------------------------------------------------------------
//  <copyright file="IUserAttach.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces
{
    public interface IDataUserProvider
    {
        object UserId { get; set; }
        
        string UserName { get; set; }
    }
}