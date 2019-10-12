// -----------------------------------------------------------------------
//  <copyright file="DatabaseChangeManager.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.Entities;
    using Microsoft.EntityFrameworkCore.Internal;
    using Specifications;

    public class DatabaseChangeManager<TContext> : IDatabaseChangeManager<TContext>
            where TContext : IApplicationContext
    {
        readonly IContextFactory<TContext> _unitOfWorkFactory;

        public DatabaseChangeManager(IContextFactory<TContext> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public DatabaseChangeCompareResult<T> Compare<T>(DataChange<T> client, DataChange<T> server, bool autoResolve)
                where T : class, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, new()
        {
            var result = new DatabaseChangeCompareResult<T>();

            var conficts = new List<ConcurrencyConflictPair>();

            foreach (var en in client.Created)
                result.Server.Created.Add(en);

            foreach (var en in server.Created)
                result.Client.Created.Add(en);

            var conflictedModify = client.Modified.Intersect(server.Modified, EntityUuidEqualityComparer<T>.Instance).ToList();

            foreach (var o in conflictedModify)
            {
                var clientSide = client.Modified.First(a => a.Uuid == o.Uuid);
                var serverSide = server.Modified.First(a => a.Uuid == o.Uuid);

                if (autoResolve)
                {
                    if (clientSide.UpdatedAt > serverSide.UpdatedAt)
                    {
                        result.Client.Modified.Add(clientSide);
                        result.Server.Modified.Add(clientSide);
                    }
                    else
                    {
                        result.Server.Modified.Add(serverSide);
                        result.Client.Modified.Add(serverSide);
                    }
                }
                else
                {
                    conficts.Add(new ConcurrencyConflictPair
                    {
                        Remote = new ConcurrencyConflictEntity { Entity = serverSide },
                        Local = new ConcurrencyConflictEntity { Entity = clientSide }
                    });
                }
            }

            foreach (var en in client.Modified.Except(server.Modified, EntityUuidEqualityComparer<T>.Instance))
                result.Server.Modified.Add(en);

            foreach (var en in client.Modified.Except(server.Modified, EntityUuidEqualityComparer<T>.Instance))
                result.Client.Modified.Add(en);

            var mergedDelete = client.Deleted.Intersect(server.Deleted, EntityUuidEqualityComparer<T>.Instance).ToList();

            foreach (var o in mergedDelete)
            {
                var clientSide = client.Deleted.First(a => a.Uuid == o.Uuid);
                var serverSide = server.Deleted.First(a => a.Uuid == o.Uuid);

                if (autoResolve)
                {
                    if (clientSide.UpdatedAt > serverSide.UpdatedAt)
                    {
                        result.Client.Deleted.Add(clientSide);
                        result.Server.Deleted.Add(clientSide);
                    }
                    else
                    {
                        result.Server.Deleted.Add(serverSide);
                        result.Client.Deleted.Add(serverSide);
                    }
                }
                else
                {
                    conficts.Add(new ConcurrencyConflictPair
                    {
                        Remote = new ConcurrencyConflictEntity { Entity = serverSide },
                        Local = new ConcurrencyConflictEntity { Entity = clientSide }
                    });
                }
            }

            foreach (var en in client.Deleted.Except(server.Deleted, EntityUuidEqualityComparer<T>.Instance))
                result.Server.Deleted.Add(en);

            foreach (var en in server.Deleted.Except(client.Deleted, EntityUuidEqualityComparer<T>.Instance))
                result.Client.Deleted.Add(en);

            result.Conflicts = conficts;

            return result;
        }

        public DateTimeOffset? GetLastActivity<T>(IEnumerable<T> data)
                where T : class, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, new()
        {
            var max = DateTimeOffset.MinValue.Ticks;

            foreach (var x in data)
            {
                var m = new[] { x.CreatedAt.Ticks, x.UpdatedAt?.Ticks ?? DateTimeOffset.MinValue.Ticks }.Max();

                if (m > max)
                    max = m;
            }

            var time = max == DateTimeOffset.MinValue.Ticks ? (DateTime?)null : new DateTime(max);

            return time;
        }

        public async Task<DataChange<T>> GetChange<T>(DateTimeOffset? lastActivityAt, IEnumerable<T> data)
                where T : class, IEntity, ICreateStampSupport, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, new()
        {
            var unit = _unitOfWorkFactory.CreateContext();
            var repo = unit.GetRepository<T>();

            var specification = new GetManySpecification<T>();

            if (lastActivityAt.HasValue)
            {
                specification.Filters.Add(a => a.CreatedAt > lastActivityAt
                                               || a.UpdatedAt != null && a.UpdatedAt > lastActivityAt);
            }

            var entities = (data ?? await repo.GetManyAsync(specification)).ToList();

            // calculates last activity from data time stamps
            var time = GetLastActivity(entities);

            var created = new List<T>();
            var modified = new List<T>();
            var deleted = new List<T>();

            foreach (var e in entities)
            {
                if (!e.DeletedFlag)
                {
                    if (!lastActivityAt.HasValue)
                        created.Add(e);
                    else if (e.UpdatedAt == null && e.CreatedAt > lastActivityAt)
                        created.Add(e);
                    else if (e.UpdatedAt != null && e.UpdatedAt > lastActivityAt)
                        modified.Add(e);
                }
                else
                {
                    if (e.UpdatedAt > lastActivityAt)
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
}