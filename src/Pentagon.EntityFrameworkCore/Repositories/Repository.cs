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
    public class Repository<TEntity> : IRepository<TEntity>
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
        
        public Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            return set.Select(entitySelector).SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            return (await set.Select(selector).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).AsReadOnly();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            var query = set.Select(selector);

            return (await query.ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).AsReadOnly();
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var set = (IQueryable<TEntity>) _set;
            
            set = specification.Apply(set);

            return PaginationHelper.CreateAsync(selector, set, specification);
        }

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