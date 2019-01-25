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
    using Microsoft.EntityFrameworkCore.Extensions.Internal;
    using Microsoft.Extensions.Logging;
    using Specifications;

    /// <summary> Represents a repository for the entity framework provider. It has similar behavior like <see cref="DbSet{TEntity}" />. Marks and gets data from database. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
            where TEntity : class, IEntity, new()
    {
        /// <summary> The inner set. </summary>
        [NotNull]
        readonly DbSet<TEntity> _set;

        [NotNull]
        readonly IQueryable<TEntity> _query;
        
        /// <summary> Initializes a new instance of the <see cref="Repository{TEntity}" /> class. </summary>
        /// <param name="logger"> The logger. </param>
        /// <param name="context"> The database context. </param>
        public Repository([NotNull] DbContext context)
        {
            DataContext = context ?? throw new ArgumentNullException(nameof(context));

            _set = DataContext.Set<TEntity>() ?? throw new ArgumentException(message: "The given entity doesn't exist in the context.");
            
            _query = _set;
        }

        /// <summary> Gets the data context. </summary>
        /// <value> The <see cref="DbContext" />. </value>
        [NotNull]
        public DbContext DataContext { get; }

        /// <inheritdoc />
        public IQueryable<TEntity> Query => _set;

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
        public Task<int> CountAsync() => _set.CountAsync();

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
            var spec = new GetOneSpecification<TEntity>(entityPredicate);

            return GetOneAsync(selector, spec);
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
        public Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector)
        {
            var spec = new GetAllSpecification<TEntity>();

            return GetAllAsync(selector, spec);
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
            
            set = specification.Apply(set);

            return await set.Select(selector).ToListAsync().ConfigureAwait(false);
        }

        #endregion

        #region GetMany

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector,
                                                       Expression<Func<TEntity, object>> orderSelector,
                                                       bool isDescending)
        {
            return GetManyAsync(e => e, entitiesSelector, orderSelector, isDescending);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                  Expression<Func<TEntity, bool>> entitiesSelector,
                                                                                  Expression<Func<TEntity, object>> orderSelector,
                                                                                  bool isDescending)
        {
            var spec = new GetManySpecification<TEntity>(entitiesSelector, orderSelector, isDescending);

            return GetManyAsync(selector, spec);
        }

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
            
            set = specification.Apply(set);

            var query = set.Select(selector);

            return await query.ToListAsync().ConfigureAwait(false);
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

            return PaginationHelper.CreateAsync(selector, set, specification);
        }

        #endregion
    }
}