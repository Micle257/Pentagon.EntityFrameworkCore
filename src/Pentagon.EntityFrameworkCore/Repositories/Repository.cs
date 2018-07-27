// -----------------------------------------------------------------------
//  <copyright file="Repository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
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
    using Collections;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Specifications;

    /// <summary> Represents a repository for the entity framework provider. It has similar behavior like <see cref="DbSet{TEntity}" />. Marks and gets data from database. </summary>
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

        [NotNull]
        readonly IQueryable<TEntity> _query;

        /// <summary> Initializes a new instance of the <see cref="Repository{TEntity}" /> class. </summary>
        /// <param name="logger"> The logger. </param>
        /// <param name="context"> The database context. </param>
        public Repository([NotNull] ILogger<Repository<TEntity>> logger,
                          [NotNull] IPaginationService paginationService,
                          [NotNull] DbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            DataContext = context ?? throw new ArgumentNullException(nameof(context));
            _set = DataContext.Set<TEntity>() ?? throw new ArgumentException(message: "The given entity doesn't exist in the context.");
            _query = _set;
        }

        /// <summary> Gets the data context. </summary>
        /// <value> The <see cref="DbContext" />. </value>
        [NotNull]
        public DbContext DataContext { get; }

        /// <inheritdoc />
        public void Forget([NotNull] TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = DataContext.Entry(entity);

            if (entry != null)
                entry.State = EntityState.Detached;
        }

        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(object id) => _set.FindAsync(id);

        /// <inheritdoc />
        public Task<TProperty> GetPropertyByForeignKeyAsync<TProperty>(object foreignKey)
                where TProperty : class => DataContext.Set<TProperty>().FindAsync(foreignKey);

        /// <inheritdoc />
        public Task<int> CountAsync() => _query.CountAsync();

        /// <inheritdoc />
        public virtual void Insert(TEntity entity)
        {
            //  Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Added)));
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
            //  Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Modified)));
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
            // Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Deleted)));
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

        #region GetOne

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector)
        {
            return GetOneAsync(e => e, entitySelector);
        }

        /// <inheritdoc />
        public Task<TSelectEntity> GetOneAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate)
        {
            var set = _set.Where(entityPredicate).Select(selector);

            return set.FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>
        {
            return GetOneAsync(e => e, specification);
        }

        public Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var set = _query;

            foreach (var include in specification.Includes)
                set = set.Include(include);

            set = specification.Apply(set);

            return set.Select(entitySelector).SingleOrDefaultAsync();
        }

        #endregion

        #region GetAll

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return GetAllAsync(e => e);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector)
        {
            var set = _query.Select(selector);

            return await set.ToListAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification)
                where TSpecification : IOrderSpecification<TEntity>
        {
            return GetAllAsync(e => e, specification);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var set = _query;

            foreach (var include in specification.Includes)
                set = set.Include(include);

            set = specification.Apply(set);

            return await set.Select(selector).ToListAsync().ConfigureAwait(false);
        }

        #endregion

        #region GetMany

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector)
        {
            return GetManyAsync(e => e, entitiesSelector);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entitiesSelector) =>
                await _query.Where(entitiesSelector).Select(selector).ToListAsync().ConfigureAwait(false);

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            return GetManyAsync(e => e, specification);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = _query;

            foreach (var include in specification.Includes)
                set = set.Include(include);

            set = specification.Apply(set);

            return await set.Select(selector).ToListAsync().ConfigureAwait(false);
        }

        #endregion

        #region GetAllPages

        public Task<IEnumerable<PagedList<TEntity>>> GetAllPagesAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> orderExpression, bool isDescendingOrder, int pageSize)
        {
            return GetAllPagesAsync(e => e, criteria, orderExpression, isDescendingOrder, pageSize);
        }

        /// <inheritdoc />
        public Task<IEnumerable<PagedList<TSelectEntity>>> GetAllPagesAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                           Expression<Func<TEntity, bool>> criteria,
                                                                                           Expression<Func<TEntity, object>> orderExpression,
                                                                                           bool isDescendingOrder,
                                                                                           int pageSize)
        {
            var specification = new GetAllPagesSpecification<TEntity>(criteria, orderExpression, isDescendingOrder, pageSize);
            return GetAllPagesAsync(selector, specification);
        }

        public Task<IEnumerable<PagedList<TEntity>>> GetAllPagesAsync<TSpecification>(TSpecification specification)
                where TSpecification : IAllPaginationSpecification<TEntity>, IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            return GetAllPagesAsync(e => e, specification);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PagedList<TSelectEntity>>> GetAllPagesAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IAllPaginationSpecification<TEntity>, IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = _query;
            var index = 1;
            var result = new List<PagedList<TSelectEntity>>();

            set = specification.Apply(set);

            while (true)
            {
                var spec = new GetPageSpecification<TEntity>(specification.Filter, specification.Order, specification.IsDescending, specification.PageSize, index);
                var list = await _paginationService.CreateAsync(selector, set, spec).ConfigureAwait(false);
                result.Add(list);
                if (!list.HasNextPage)
                    break;
                index++;
            }

            return result;
        }

        #endregion

        #region GetPage

        public Task<PagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex)
        {
            return GetPageAsync(e => e, criteria, order, isDescendingOrder, pageSize, pageIndex);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                          Expression<Func<TEntity, bool>> criteria,
                                                                          Expression<Func<TEntity, object>> order,
                                                                          bool isDescendingOrder,
                                                                          int pageSize,
                                                                          int pageIndex)
        {
            var specification = new GetPageSpecification<TEntity>(criteria, order, isDescendingOrder, pageSize, pageIndex);
            return GetPageAsync(selector, specification);
        }

        public Task<PagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            return GetPageAsync(e => e, specification);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var set = _query;
            set = specification.Apply(set);
            return _paginationService.CreateAsync(selector, set, specification);
        }

        #endregion
    }
}