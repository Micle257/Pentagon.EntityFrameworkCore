namespace Pentagon.EntityFrameworkCore.Synchronization {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Specifications;

    public class DatabaseChangeCompareResult<TEntity>
    {
        public DataChange<TEntity> Client { get; set; }

        public DataChange<TEntity> Server { get; set; }

        public IReadOnlyList<ConcurrencyConflictPair> Conflicts { get; set; }
    }

    public class DatabaseChangeManager<TContext>
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
            {
                result.Server.Created.Add(en);
            }

            foreach (var en in server.Created)
            {
                result.Client.Created.Add(en);
            }

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
            {
                result.Server.Modified.Add(en);
            }

            return result;
        }

        public async Task<DataChange<T>> GetChange<T>(DateTimeOffset? lastActivityAt, IEnumerable<T> data)
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
            var entities = data ?? await repo.GetManyAsync(specification);

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