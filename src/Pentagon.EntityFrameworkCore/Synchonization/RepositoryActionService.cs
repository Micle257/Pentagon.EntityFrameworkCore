namespace Pentagon.Data.EntityFramework.Synchonization {
    using System;
    using System.Collections.Generic;
    using Abstractions.Entities;
    using Pentagon.Extensions.DependencyInjection;

    [Register(RegisterType.Transient, typeof(IRepositoryActionService))]
    public class RepositoryActionService : IRepositoryActionService
    {
        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInOneWayMode<TEntity>(EntityPair<TEntity> pair)
                where TEntity : class, IEntity, ITimeStampSupport, ICreateStampSupport
        {
            if (pair.Type == EntityPairType.Both && pair.Remote.CreateGuid != pair.Local.CreateGuid)
                throw new ArgumentException("The given entities are not created from the same source.");

            var comms = new List<RepositoryAction<TEntity>>();

            switch (pair.Type)
            {
                case EntityPairType.LocalOnly:
                    comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Local, TableActionType.Delete));
                    break;

                case EntityPairType.RemoteOnly:
                    comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Remote, TableActionType.Insert));
                    break;

                case EntityPairType.Both:
                    if (pair.Remote.LastUpdatedAt > pair.Local.LastUpdatedAt)
                    {
                        comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Remote, TableActionType.Update));
                    }
                    break;
            }

            return comms;
        }

        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInOneWayMode<TEntity>(TEntity remoteEntity, TEntity localEntity)
                where TEntity : class, IEntity, ITimeStampSupport, ICreateStampSupport
        {
            return GetRepositoryActionsInOneWayMode(new EntityPair<TEntity>(localEntity, remoteEntity));
        }

        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInTwoWayMode<TEntity>(TEntity remoteEntity, TEntity localEntity)
                where TEntity : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport
        {
            return GetRepositoryActionsInTwoWayMode(new EntityPair<TEntity>(localEntity, remoteEntity));
        }

        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInTwoWayMode<TEntity>(EntityPair<TEntity> pair)
                where TEntity : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport
        {
            if (pair.Type == EntityPairType.Both && pair.Remote.CreateGuid != pair.Local.CreateGuid)
                throw new ArgumentException("The given entities are not created from the same source.");

            var comms = new List<RepositoryAction<TEntity>>();

            switch (pair.Type)
            {
                case EntityPairType.RemoteOnly:
                    if (pair.Local.IsDeletedFlag)
                        comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Local, TableActionType.Delete));
                    else
                        comms.Add(new RepositoryAction<TEntity>(RepositoryType.Remote, pair.Local, TableActionType.Insert));
                    break;
                case EntityPairType.LocalOnly:
                    if (!pair.Remote.IsDeletedFlag)
                        comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Remote, TableActionType.Insert));
                    break;
                case EntityPairType.Both:
                    if (pair.Remote.LastUpdatedAt > pair.Local.LastUpdatedAt)
                    {
                        if (pair.Remote.IsDeletedFlag)
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Local, TableActionType.Delete));
                        else
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Remote, TableActionType.Update));
                    }
                    else if (pair.Remote.LastUpdatedAt < pair.Local.LastUpdatedAt)
                    {
                        if (pair.Remote.IsDeletedFlag)
                        {
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Remote, pair.Local, TableActionType.Update));
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Local, TableActionType.Delete));
                        }
                        else
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Remote, pair.Local, TableActionType.Update));
                    }

                    break;
            }

            foreach (var c in comms)
                c.Entity.Id = default(int);

            return comms;
        }
    }
}