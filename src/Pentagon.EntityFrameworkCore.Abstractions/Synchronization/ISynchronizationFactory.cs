// -----------------------------------------------------------------------
//  <copyright file="ISynchronizationFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using Abstractions.Entities;

    public interface ISynchronizationFactory
    {
        ITwoWaySynchronization<T> CreateTwoWay<T>()
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new();
    }
}