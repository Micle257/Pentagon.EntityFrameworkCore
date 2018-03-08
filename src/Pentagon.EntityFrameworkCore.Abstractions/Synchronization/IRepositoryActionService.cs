namespace Pentagon.Data.EntityFramework.Synchonization {
    using System.Collections.Generic;
    using Abstractions.Entities;

    public interface IRepositoryActionService {
        IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInOneWayMode<TEntity>(EntityPair<TEntity> pair)
                where TEntity : class, IEntity, ITimeStampSupport, ICreateStampSupport;

        IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInOneWayMode<TEntity>(TEntity remoteEntity, TEntity localEntity)
                where TEntity : class, IEntity, ITimeStampSupport, ICreateStampSupport;

        IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInTwoWayMode<TEntity>(TEntity remoteEntity, TEntity localEntity)
                where TEntity : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport;

        IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInTwoWayMode<TEntity>(EntityPair<TEntity> pair)
                where TEntity : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport;
    }
}