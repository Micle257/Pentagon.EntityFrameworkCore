// -----------------------------------------------------------------------
//  <copyright file="ISynchronizationFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using Interfaces.Entities;

    public interface ISynchronizationFactory
    {
        ISynchronization<T> Create<T>()
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new();
    }
}