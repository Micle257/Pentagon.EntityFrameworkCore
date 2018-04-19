// -----------------------------------------------------------------------
//  <copyright file="ISynchronizationSupport.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Entities
{
    /// <summary>
    /// Represent an entity, that supports one way database synchronization.
    /// </summary>
    public interface IOneWaySynchronizationSupport : ITimeStampSupport, ICreateStampSupport { }
}