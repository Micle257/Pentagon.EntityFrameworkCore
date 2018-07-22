// -----------------------------------------------------------------------
//  <copyright file="IOneWaySynchronizationSupport.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    /// <summary> Represent an entity, that supports one way database synchronization. </summary>
    public interface IOneWaySynchronizationSupport : ICreatedTimeStampSupport, IUpdatedTimeStampSupport, ICreateStampSupport { }
}