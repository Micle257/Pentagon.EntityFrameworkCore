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
    using Abstractions;
    using Abstractions.Entities;
    using Specifications;

    public class DatabaseChangeManager<TContext> : IDatabaseChangeManager<TContext>
            where TContext : IApplicationContext
    {
        readonly IUnitOfWorkFactory<TContext> _unitOfWorkFactory;

        public DatabaseChangeManager(IUnitOfWorkFactory<TContext> unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public DatabaseChangeCompareResult<T> Compare<T>(DataChange<T> client, DataChange<T> server)
                where T : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
        {
            var result = new DatabaseChangeCompareResult<T>();

            var conflicts = new List<ConcurrencyConflictPair>();

            foreach (var en in client.Created)
                result.Server.Created.Add(en);

            foreach (var en in server.Created)
                result.Client.Created.Add(en);

            var conflictedModify = client.Modified.Select(a => a.Id).Intersect(server.Modified.Select(a => a.Id));

            foreach (var o in conflictedModify)
            {
                conflicts.Add(new ConcurrencyConflictPair
                              {
                                      FromDatabase = new ConcurrencyConflictEntity
                                                     {
                                                             Entity = server.Modified.FirstOrDefault(a => a.Id == o)
                                                     },
                                      Posted = new ConcurrencyConflictEntity
                                               {
                                                       Entity = client.Modified.FirstOrDefault(a => a.Id == o)
                                               }
                              });
            }

            foreach (var en in client.Modified.Where(a => server.Modified.Any(b => b.Id != a.Id)))
                result.Server.Modified.Add(en);

            foreach (var en in server.Modified.Where(a => client.Modified.Any(b => b.Id != a.Id)))
                result.Client.Modified.Add(en);

            var conflictedDelete = client.Deleted.Select(a => a.Id).Intersect(server.Deleted.Select(a => a.Id));

            foreach (var o in conflictedDelete)
            {
                conflicts.Add(new ConcurrencyConflictPair
                              {
                                      FromDatabase = new ConcurrencyConflictEntity
                                                     {
                                                             Entity = server.Deleted.FirstOrDefault(a => a.Id == o)
                                                     },
                                      Posted = new ConcurrencyConflictEntity
                                               {
                                                       Entity = client.Deleted.FirstOrDefault(a => a.Id == o)
                                               }
                              });
            }

            foreach (var en in client.Deleted.Where(a => server.Deleted.Any(b => b.Id != a.Id)))
                result.Server.Deleted.Add(en);

            foreach (var en in server.Deleted.Where(a => client.Deleted.Any(b => b.Id != a.Id)))
                result.Client.Deleted.Add(en);

            return result;
        }

        public DateTimeOffset? GetLastActivity<T>(IEnumerable<T> data)
                where T : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
        {
            var max = DateTimeOffset.MinValue.Ticks;

            foreach (var x in data)
            {
                var m = new[] {x.CreatedAt.Ticks, x.UpdatedAt?.Ticks ?? DateTimeOffset.MinValue.Ticks, x.DeletedAt?.Ticks ?? DateTimeOffset.MinValue.Ticks}.Max();

                if (m > max)
                    max = m;
            }

            var time = max == DateTimeOffset.MinValue.Ticks ? (DateTime?) null : new DateTime(max);

            return time;
        }

        public async Task<DataChange<T>> GetChange<T>(DateTimeOffset? lastActivityAt, IEnumerable<T> data)
                where T : class, IEntity, ICreateTimeStampSupport, IUpdateTimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new()
        {
            var unit = _unitOfWorkFactory.Create();
            var repo = unit.GetRepository<T>();

            var specification = new GetManySpecification<T>();

            if (lastActivityAt.HasValue)
            {
                specification.Filters.Add(a => a.CreatedAt > lastActivityAt
                                               || a.UpdatedAt != null && a.UpdatedAt > lastActivityAt
                                               || a.DeletedAt != null && a.DeletedAt > lastActivityAt);
            }

            var entities = (data ?? await repo.GetManyAsync(specification)).ToList();

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
}