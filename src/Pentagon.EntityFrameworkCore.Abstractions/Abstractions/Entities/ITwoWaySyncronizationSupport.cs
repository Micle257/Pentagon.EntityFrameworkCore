// -----------------------------------------------------------------------
//  <copyright file="ISynchronizationSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Abstractions.Entities
{
    /// <summary>
    /// Represent an entity, that supports one way database synchronization.
    /// </summary>
    public interface IOneWaySynchronizationSupport : ITimeStampSupport, ICreateStampSupport { }

    /// <summary>
    /// Represent an entity, that supports two way database synchronization.
    /// </summary>
    public interface ITwoWaySynchronizationSupport : ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, ICreateStampSupport { }
}