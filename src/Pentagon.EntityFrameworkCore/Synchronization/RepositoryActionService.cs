// -----------------------------------------------------------------------
//  <copyright file="RepositoryActionService.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Collections.Generic;
    using Abstractions.Entities;

    public class RepositoryActionService : IRepositoryActionService
    {
        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInOneWayMode<TEntity>(EntityPair<TEntity> pair)
                where TEntity : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, ICreateStampSupport
        {
            if (pair.Type == EntityPairType.Both && pair.Remote.Uuid != pair.Local.Uuid)
                throw new ArgumentException(message: "The given entities are not created from the same source.");

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
                    if (pair.Remote.UpdatedAt > pair.Local.UpdatedAt)
                        comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Remote, TableActionType.Update));
                    break;
            }

            return comms;
        }

        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInOneWayMode<TEntity>(TEntity remoteEntity, TEntity localEntity)
                where TEntity : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, ICreateStampSupport => GetRepositoryActionsInOneWayMode(new EntityPair<TEntity>(localEntity, remoteEntity));

        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInTwoWayMode<TEntity>(TEntity remoteEntity, TEntity localEntity)
                where TEntity : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport =>
                GetRepositoryActionsInTwoWayMode(new EntityPair<TEntity>(localEntity, remoteEntity));

        public IEnumerable<RepositoryAction<TEntity>> GetRepositoryActionsInTwoWayMode<TEntity>(EntityPair<TEntity> pair)
                where TEntity : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport
        {
            if (pair.Type == EntityPairType.Both && pair.Remote.Uuid != pair.Local.Uuid)
                throw new ArgumentException(message: "The given entities are not created from the same source.");

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
                    if (pair.Remote.UpdatedAt > pair.Local.UpdatedAt)
                    {
                        if (pair.Remote.IsDeletedFlag)
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Local, TableActionType.Delete));
                        else
                            comms.Add(new RepositoryAction<TEntity>(RepositoryType.Local, pair.Remote, TableActionType.Update));
                    }
                    else if (pair.Remote.UpdatedAt < pair.Local.UpdatedAt)
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