// -----------------------------------------------------------------------
//  <copyright file="TwoWaySynchronization.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
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
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using Specifications;

    public class DataChange<T>
    {
        public DateTimeOffset? LastActivityAt { get; set; }

        public IEnumerable<T> Created { get; set; }

        public IEnumerable<T> Modified { get; set; }

        public IEnumerable<T> Deleted { get; set; }
    }

    public class DatabaseChangeManager<TContext>
            where TContext : IApplicationContext
    {
        readonly IUnitOfWorkFactory<TContext> _unitOfWorkFactory;

        public DatabaseChangeManager(IUnitOfWorkFactory<TContext> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<DataChange<T>> GetChange<T>(DateTimeOffset? lastActivityAt)
            where T : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
        {
            var unit = _unitOfWorkFactory.Create();
            var repo = unit.GetRepository<T>();

            var specification = new GetManySpecification<T>();
            
            if (lastActivityAt.HasValue)
            specification.Filters.Add(a => (a.CreatedAt > lastActivityAt) 
                                           || (a.UpdatedAt != null  && a.UpdatedAt > lastActivityAt) 
                                           || ( a.DeletedAt != null && a.DeletedAt > lastActivityAt));
           // createSpecification.Filters.Add(a => !(a as IDeletedFlagSupport).IsDeletedFlag);
           // 
           // modifySpecification.Filters.Add(a => a.UpdatedAt != null  && a.UpdatedAt > lastActivityAt);
           // modifySpecification.Filters.Add(a => !(a as IDeletedFlagSupport).IsDeletedFlag);
           //
           // deleteSpecification.Filters.Add(a => (a as IDeletedFlagSupport).IsDeletedFlag && a.DeletedAt != null);
           // deleteSpecification.Filters.Add(a => (a as IDeleteTimeStampSupport).DeletedAt > lastActivityAt);

            var time = DateTimeOffset.Now;
            var entities = await repo.GetManyAsync(specification);

            var created = new List<T>();
            var modified = new List<T>();
            var deleted = new List<T>();

            foreach (var e in entities)
            {
                if (!e.IsDeletedFlag)
                {
                    if (e.UpdatedAt == null && e.CreatedAt > lastActivityAt)
                        created.Add(e);
                    else if (e.UpdatedAt != null && e.UpdatedAt > lastActivityAt)
                        modified.Add(e);
                }
                else
                {
                    if (e.DeletedAt > lastActivityAt)
                        deleted.Add(e);
                }
            }

            return new DataChange<T>
                   {
                           LastActivityAt = time,
                           Created = created,
                           Deleted = deleted,
                           Modified = modified
                   };
        }
    }

    /// <summary> Represents a synchronization session. </summary>
    /// <typeparam name="T"> The type of the entity. </typeparam>
    public class TwoWaySynchronization<T> : ITwoWaySynchronization<T>
            where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
    {
        readonly IRepositoryActionService _actionService;

        readonly IUnitOfWorkScope<IRemoteContext> _remoteFactory;

        readonly IUnitOfWorkScope<ILocalContext> _localFactory;

        public TwoWaySynchronization(IRepositoryActionService actionService,
                                     IUnitOfWorkScope<IRemoteContext> remoteFactory,
                                     IUnitOfWorkScope<ILocalContext> localFactory)
        {
            _actionService = actionService;
            _remoteFactory = remoteFactory;
            _localFactory = localFactory;
        }

        public async Task SynchronizeAsync(Expression<Func<T, bool>> selector)
        {
            using (_localFactory)
            {
                var local = _localFactory.Get();

                using (_remoteFactory)
                {
                    var remote = _remoteFactory.Get();

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