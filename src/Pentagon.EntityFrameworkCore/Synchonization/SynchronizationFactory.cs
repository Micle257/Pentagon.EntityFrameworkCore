namespace Pentagon.Data.EntityFramework.Synchonization {
    using Abstractions;
    using Abstractions.Entities;
    using Pentagon.Extensions.DependencyInjection;

    [Register(RegisterType.Transient, typeof(ISynchronizationFactory))]
    public class SynchronizationFactory : ISynchronizationFactory
    {
        readonly IRepositoryActionService _actionService;
        readonly IUnitOfWorkFactory<IRemoteContext> _remote;
        readonly IUnitOfWorkFactory<ILocalContext> _local;

        public SynchronizationFactory(IRepositoryActionService actionService, IUnitOfWorkFactory<IRemoteContext> remote, IUnitOfWorkFactory<ILocalContext> local)
        {
            _actionService = actionService;
            _remote = remote;
            _local = local;
        }

        public ITwoWaySynchronization<T> CreateTwoWay<T>()
                where T : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
        {
            return new TwoWaySynchronization<T>(_actionService, _remote, _local);
        }
    }
}