// -----------------------------------------------------------------------
//  <copyright file="TwoWaySynchronization.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.Entities;
    using Interfaces.Repositories;
    using Repositories;
    using Specifications;

    /// <summary> Represents a synchronization session. </summary>
    /// <typeparam name="T"> The type of the entity. </typeparam>
    public class Synchronization<T> : ISynchronization<T>
            where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
    {
        readonly IRepositoryActionService _actionService;

        readonly IContextFactory _remoteFactory;

        readonly IContextFactory _localFactory;

        public Synchronization(IRepositoryActionService actionService,
                                     IContextFactory remoteFactory,
                               IContextFactory localFactory)
        {
            _actionService = actionService;
            _remoteFactory = remoteFactory;
            _localFactory = localFactory;
        }

        public async Task SynchronizeAsync(Expression<Func<T, bool>> selector)
        {
            var local = _localFactory.CreateContext();
            var remote = _remoteFactory.CreateContext();

            var localRepository = local.GetRepository<T>();
            var remoteRepository = remote.GetRepository<T>();

            var dataDiff = await GetDataPairsAsync(localRepository, remoteRepository, selector).ConfigureAwait(false);

            foreach (var diff in dataDiff)
            {
                var comms = _actionService.GetRepositoryActionsInOneWayMode(diff);
                foreach (var comm in comms)
                {
                    var repo = comm.RepositoryType == RepositoryType.Local
                                       ? localRepository
                                       : remoteRepository;

                    switch (comm.Action)
                    {
                        case TableActionType.Insert:
                            repo.Insert(comm.Entity);
                            break;
                        case TableActionType.Delete:
                            repo.Delete(comm.Entity);
                            break;
                        case TableActionType.Update:
                            repo.Update(comm.Entity);
                            break;
                    }
                }
            }
        }

        /// <summary> Gets the remote/local entity pairs. </summary>
        /// <param name="selector"> The selector for getting the data. </param>
        /// <returns> An awaitable list of the <see cref="EntityPair{T}" />. </returns>
        public async Task<IList<EntityPair<T>>> GetDataPairsAsync(IRepository<T> localRepository, IRepository<T> remoteRepository, Expression<Func<T, bool>> selector)
        {
            var result = new List<EntityPair<T>>();

            var specification = new GetManySpecification<T>(selector, arg => arg.Id, true);

            var remoteData = (await remoteRepository.GetManyAsync(specification).ConfigureAwait(false)).ToList();
            var localData = (await localRepository.GetManyAsync(specification).ConfigureAwait(false)).ToList();

            var intersect = remoteData.Select(v => v.Uuid).Intersect(localData.Select(v => v.Uuid)).ToList();

            var diffRemote = remoteData.Select(v => v.Uuid).Except(intersect);
            var diffLocal = localData.Select(v => v.Uuid).Except(intersect);

            result.AddRange(intersect.Select(time => (remoteData.FirstOrDefault(v => v.Uuid == time), localData.FirstOrDefault(v => v.Uuid == time)))
                                     .Select(tuple => new EntityPair<T>(tuple.Item1, tuple.Item2)));

            result.AddRange(diffRemote.Select(time => remoteData.FirstOrDefault(v => v.Uuid == time))
                                      .Select(remote => new EntityPair<T>(remote, null)));

            result.AddRange(diffLocal.Select(time => localData.FirstOrDefault(v => v.Uuid == time))
                                     .Select(local => new EntityPair<T>(null, local)));

            return result;
        }
    }
}