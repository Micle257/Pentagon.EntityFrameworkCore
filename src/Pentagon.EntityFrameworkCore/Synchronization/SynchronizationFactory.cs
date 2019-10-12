// -----------------------------------------------------------------------
//  <copyright file="SynchronizationFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using Interfaces;
    using Interfaces.Entities;

    public class SynchronizationFactory<TRemoteContext,TLocalContext> : ISynchronizationFactory
            where TRemoteContext : IApplicationContext
            where TLocalContext : IApplicationContext
    {
        readonly IRepositoryActionService _actionService;
        readonly IContextFactory<TRemoteContext> _remote;
        readonly IContextFactory<TLocalContext> _local;

        public SynchronizationFactory(IRepositoryActionService actionService, IContextFactory<TRemoteContext> remote, IContextFactory<TLocalContext> local)
        {
            _actionService = actionService;
            _remote = remote;
            _local = local;
        }

        public ISynchronization<T> Create<T>()
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new() 
            => new Synchronization<T>(_actionService, _remote, _local);
    }
}