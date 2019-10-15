// -----------------------------------------------------------------------
//  <copyright file="Repository.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Interfaces.Entities;
    using Interfaces.Repositories;
    using Interfaces.Specifications;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Specifications;

    /// <summary> Represents a repository for the entity framework provider. It has similar behavior like <see cref="DbSet{TEntity}" />. Marks and gets data from database. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class Repository<TEntity> : IRepositoryTransient<TEntity>
            where TEntity : class, IEntity, new()
    {
        /// <summary> The inner set. </summary>
        [NotNull]
        readonly DbSet<TEntity> _set;
        
        /// <summary> Initializes a new instance of the <see cref="Repository{TEntity}" /> class. </summary>
        /// <param name="logger"> The logger. </param>
        /// <param name="context"> The database context. </param>
        public Repository([NotNull] DbSet<TEntity> dbSet)
        {
            _set = dbSet;
        }
        
        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default) => _set.FindAsync( new [] {id}, cancellationToken).AsTask();
        
        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken = default) => _set.CountAsync(cancellationToken);

        /// <inheritdoc />
        public virtual void Insert(TEntity entity)
        {
            //  Commiting?.Invoke(this, new CommitEventArgs(new Entry(entity, EntityStateType.Added)));
            _set.Add(entity);
        }

        /// <inheritdoc />
        public virtual void InsertMany([NotNull] params TEntity[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            
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
        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector, CancellationToken cancellationToken = default)
        {
            return GetOneAsync(e => e, entitySelector, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TSelectEntity> GetOneAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate, CancellationToken cancellationToken = default)
        {
            var spec = new GetOneSpecification<TEntity>(entityPredicate);

            return GetOneAsync(selector, spec, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            return GetOneAsync(e => e, specification, cancellationToken);
        }

        public Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            return set.Select(entitySelector).SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }

        #endregion

        #region GetAll

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return GetAllAsync(e => e, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, CancellationToken cancellationToken = default)
        {
            var spec = new GetAllSpecification<TEntity>();

            return GetAllAsync(selector, spec, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            return GetAllAsync(e => e, specification, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            return await set.Select(selector).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region GetMany

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector,
                                                       Expression<Func<TEntity, object>> orderSelector,
                                                       bool isDescending,
                                                       CancellationToken cancellationToken = default)
        {
            return GetManyAsync(e => e, entitiesSelector, orderSelector, isDescending, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                  Expression<Func<TEntity, bool>> entitiesSelector,
                                                                                  Expression<Func<TEntity, object>> orderSelector,
                                                                                  bool isDescending,
                                                                                  CancellationToken cancellationToken = default)
        {
            var spec = new GetManySpecification<TEntity>(entitiesSelector, orderSelector, isDescending);

            return GetManyAsync(selector, spec, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            return GetManyAsync(e => e, specification, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            var query = set.Select(selector);

            return await query.ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        #endregion
        
        #region GetPage

        public Task<PagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex, CancellationToken cancellationToken = default)
        {
            return GetPageAsync(e => e, criteria, order, isDescendingOrder, pageSize, pageIndex, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                          Expression<Func<TEntity, bool>> criteria,
                                                                          Expression<Func<TEntity, object>> order,
                                                                          bool isDescendingOrder,
                                                                          int pageSize,
                                                                          int pageIndex,
                                                                          CancellationToken cancellationToken = default)
        {
            var specification = new GetPageSpecification<TEntity>(criteria, order, isDescendingOrder, pageSize, pageIndex);

            return GetPageAsync(selector, specification, cancellationToken);
        }

        public Task<PagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            return GetPageAsync(e => e, specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            return PaginationHelper.CreateAsync(selector, set, specification);
        }

        #endregion

        /// <inheritdoc />
        public IEnumerator<TEntity> GetEnumerator() => _set.AsQueryable().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public Type ElementType => _set.AsQueryable().ElementType;

        /// <inheritdoc />
        public Expression Expression => _set.AsQueryable().Expression;

        /// <inheritdoc />
        public IQueryProvider Provider => _set.AsQueryable().Provider;
    }
}