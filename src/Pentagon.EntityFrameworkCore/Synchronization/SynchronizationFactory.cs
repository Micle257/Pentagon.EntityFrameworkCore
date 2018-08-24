// -----------------------------------------------------------------------
//  <copyright file="SynchronizationFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using Abstractions;
    using Abstractions.Entities;

    public class SynchronizationFactory<TRemoteContext,TLocalContext> : ISynchronizationFactory
            where TRemoteContext : IApplicationContext
            where TLocalContext : IApplicationContext
    {
        readonly IRepositoryActionService _actionService;
        readonly IUnitOfWorkScope<TRemoteContext> _remote;
        readonly IUnitOfWorkScope<TLocalContext> _local;

        public SynchronizationFactory(IRepositoryActionService actionService, IUnitOfWorkScope<TRemoteContext> remote, IUnitOfWorkScope<TLocalContext> local)
        {
            _actionService = actionService;
            _remote = remote;
            _local = local;
        }

        public ITwoWaySynchronization<T> CreateTwoWay<T>()
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new() 
            => new TwoWaySynchronization<T>(_actionService, _remote, _local);
    }
}