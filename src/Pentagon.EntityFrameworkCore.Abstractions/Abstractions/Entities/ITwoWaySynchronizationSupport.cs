// -----------------------------------------------------------------------
//  <copyright file="ITwoWaySynchronizationSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    /// <summary> Represent an entity, that supports two way database synchronization. </summary>
    public interface ITwoWaySynchronizationSupport : ICreatedTimeStampSupport, IUpdatedTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, ICreateStampSupport { }
}