// -----------------------------------------------------------------------
//  <copyright file="Repository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.Data.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using Abstractions.Specifications;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary> Represents a repository for the entity framework provider. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        /// <summary> The logger. </summary>
        [NotNull]
        readonly ILogger<Repository<TEntity>> _logger;

        [NotNull]
        readonly IPaginationService _paginationService;

        /// <summary> The inner set. </summary>
        [NotNull]
        readonly DbSet<TEntity> _set;

        IQueryable<TEntity> _currectQuery;

        /// <summary> Initializes a new instance of the <see cref="Repository{TEntity}" /> class. </summary>
        /// <param name="logger"> The logger. </param>
        /// <param name="context"> The database context. </param>
        public Repository([NotNull] ILogger<Repository<TEntity>> logger, [NotNull] IPaginationService paginationService, [NotNull] DbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            DataContext = context ?? throw new ArgumentNullException(nameof(context));
            _set = DataContext.Set<TEntity>() ?? throw new ArgumentException(message: "The given entity doesn't exist in the context.");
        }

        /// <inheritdoc />
        public event EventHandler<CommitEventArgs> Commiting;

        /// <summary> Gets the data context. </summary>
        /// <value> The <see cref="ApplicationContext" />. </value>
        [NotNull]
        public DbContext DataContext { get; }

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector, bool trackChanges = false)
        {
            var q = GetPreQuery();
            var set = trackChanges ? q.AsNoTracking() : q;
            return set.FirstOrDefaultAsync(entitySelector);
        }

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification)
            where TSpecification : ICriteriaSpecification<TEntity>
        {
            var set = GetPreQuery();

            foreach (var include in specification.Includes)
                set = set.Include(include);

            return specification.Apply(set).SingleOrDefaultAsync();
        }

        /// <inheritdoc />
        public virtual Task<TEntity> GetByIdAsync(object id) => _set.FindAsync(id);

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
        {
            var q = GetPreQuery();
            var set = asNoTracking ? q.AsNoTracking() : q;
            return await set.ToListAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification)
            where TSpecification : IOrderSpecification<TEntity>
        {
            var set = GetPreQuery();

            foreach (var include in specification.Includes)
                set = set.Include(include);

            return await specification.Apply(set).ToListAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification)
            where TSpecification : ICriteriaSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = GetPreQuery();

            foreach (var include in specification.Includes)
                set = set.Include(include);

            return await specification.Apply(set).ToListAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<TProperty> GetPropertyByForeignKeyAsync<TProperty>(object foreignKey)
            where TProperty : class => DataContext.Set<TProperty>().FindAsync(foreignKey);

        /// <inheritdoc />
        public Task<int> CountAsync()
        {
            var q = GetPreQuery();
            return q.Select(a => a.Id).CountAsync();
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector, bool asNoTracking = false)
        {
            var q = GetPreQuery();
            var set = asNoTracking ? q.AsNoTracking() : q;
            return await set.Where(entitiesSelector).ToListAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual void Insert(TEntity entity)
        {
            Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Added)));
            _set.Add(entity);
        }

        /// <inheritdoc />
        public virtual void Insert<TUserId>(TEntity entity, TUserId userId)
        {
            Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Added, userId)));
            _set.Add(entity);
        }

        /// <inheritdoc />
        public virtual void InsertMany(params TEntity[] entities)
        {
            Require.NotNull(() => entities);
            Require.Condition(() => entities, en => en.Length > 0, message: "The length of entities must be greaten than zero. Use basic insert instead.");

            foreach (var e in entities)
                Insert(e);
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Modified)));
            _set.Update(entity);
        }

        /// <inheritdoc />
        public void Update<TUserId>(TEntity entity, TUserId userId)
        {
            Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Modified, userId)));
            _set.Update(entity);
        }

        /// <inheritdoc />
        public void UpdateMany(params TEntity[] entities)
        {
            foreach (var entity in entities)
                Update(entity);
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity entity)
        {
            Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Deleted)));
            _set.Remove(entity);
        }

        /// <inheritdoc />
        public void Delete<TUserId>(TEntity entity, TUserId userId)
        {
            Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Deleted, userId)));
            _set.Remove(entity);
        }

        /// <inheritdoc />
        public void DeleteMany(params TEntity[] entities)
        {
            foreach (var entity in entities)
                Delete(entity);
        }

        /// <inheritdoc />
        public void Truncate()
        {
            _set.RemoveRange(_set);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IPagedList<TEntity>>> GetAllPagesAsync(Expression<Func<TEntity, bool>> criteria,
                                                                       Expression<Func<TEntity, object>> orderExpression,
                                                                       bool isDescendingOrder,
                                                                       int pageSize)
        {
            var specification = new GetAllPagesSpecification<TEntity>(criteria, orderExpression, isDescendingOrder, pageSize);
            return GetAllPagesAsync(specification);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IPagedList<TEntity>>> GetAllPagesAsync<TSpecification>(TSpecification specification)
            where TSpecification : IAllPaginationSpecification<TEntity>, ICriteriaSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = GetPreQuery();
            var index = 1;
            var result = new List<IPagedList<TEntity>>();
            set = specification.Apply(set);
            while (true)
            {
                var spec = new GetPageSpecification<TEntity>(specification.Criteria, specification.Order, specification.IsDescending, specification.PageSize, index);
                var list = await _paginationService.CreateAsync(set, spec).ConfigureAwait(false);
                result.Add(list);
                if (!list.HasNextPage)
                    break;
                index++;
            }
            return result;
        }

        /// <inheritdoc />
        public Task<IPagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification)
            where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, ICriteriaSpecification<TEntity>
        {
            var set = GetPreQuery();
            set = specification.Apply(set);
            return _paginationService.CreateAsync(set, specification);
        }

        /// <inheritdoc />
        public Task<IPagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex)
        {
            var specification = new GetPageSpecification<TEntity>(criteria, order, isDescendingOrder, pageSize, pageIndex);
            return GetPageAsync(specification);
        }

        public IRepository<TEntity> Query(Func<IQueryable<TEntity>, IQueryable<TEntity>> function)
        {
            if (_currectQuery == null)
                _currectQuery = _set;
            _currectQuery = function(_currectQuery);
            return this;
        }

        IQueryable<TEntity> GetPreQuery()
        {
            if (_currectQuery == null)
                return _set;

            var q = _currectQuery;
            _currectQuery = null;
            return q;
        }
    }
}