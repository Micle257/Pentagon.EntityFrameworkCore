// -----------------------------------------------------------------------
//  <copyright file="SynchronizationFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using Abstractions;
    using Abstractions.Entities;

    public class SynchronizationFactory : ISynchronizationFactory
    {
        readonly IRepositoryActionService _actionService;
        readonly IUnitOfWorkScope<IRemoteContext> _remote;
        readonly IUnitOfWorkScope<ILocalContext> _local;

        public SynchronizationFactory(IRepositoryActionService actionService, IUnitOfWorkScope<IRemoteContext> remote, IUnitOfWorkScope<ILocalContext> local)
        {
            _actionService = actionService;
            _remote = remote;
            _local = local;
        }

        public ITwoWaySynchronization<T> CreateTwoWay<T>()
                where T : class, IEntity, ICreateStampSupport, ICreatedTimeStampSupport, IUpdatedTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new() => new TwoWaySynchronization<T>(_actionService, _remote, _local);
    }
}