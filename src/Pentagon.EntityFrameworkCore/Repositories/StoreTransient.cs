﻿// -----------------------------------------------------------------------
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
    using Interfaces;
    using Interfaces.Entities;
    using Interfaces.Repositories;
    using Interfaces.Specifications;
    using Interfaces.Stores;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Specifications;

    /// <summary> Represents a repository for the entity framework provider. It has similar behavior like <see cref="DbSet{TEntity}" />. Marks and gets data from database. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class StoreTransient<TEntity> : IStoreTransient<TEntity>
            where TEntity : class, IEntity, new()
    {
        [NotNull]
        readonly IApplicationContext _context;
        
        /// <summary> Initializes a new instance of the <see cref="Repository{TEntity}" /> class. </summary>
        /// <param name="context"> The database context. </param>
        public StoreTransient([NotNull] IApplicationContext context)
        {
            _context = context;
        }
        
        /// <inheritdoc />
        public Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetByIdAsync(new[] {id}, cancellationToken);
        }

        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult> DeleteAsync(TEntity entity)
        {
            try
            {
                var repository = _context.GetRepository<TEntity>();

                repository.Delete(entity);

                var result = await _context.ExecuteCommitAsync();

                return result;
            }
            catch (Exception e)
            {
                return new ContextCommitResult
                       {
                               Exception = e
                       };
            }
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult> DeleteManyAsync(params TEntity[] entities)
        {
            try
            {
                var repository = _context.GetRepository<TEntity>();

                repository.DeleteMany(entities);

                var result = await _context.ExecuteCommitAsync();

                return result;
            }
            catch (Exception e)
            {
                return new ContextCommitResult
                       {
                               Exception = e
                       };
            }
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<TEntity>> InsertAsync(TEntity entity)
        {
            try
            {
                var repository = _context.GetRepository<TEntity>();

                repository.Insert(entity);

                var result = await _context.ExecuteCommitAsync();

                return new ContextCommitResult<TEntity>
                       {
                               Result = entity,
                               Exception = result.Exception,
                               Conflicts = result.Conflicts,
                               Content = result.Content
                       };
            }
            catch (Exception e)
            {
                return new ContextCommitResult<TEntity>
                {
                               Exception = e
                       };
            }
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> InsertManyAsync(params TEntity[] entities)
        {
            try
            {
                var repository = _context.GetRepository<TEntity>();

                repository.InsertMany(entities);

                var result = await _context.ExecuteCommitAsync();

                return new ContextCommitResult<IReadOnlyCollection<TEntity>>
                       {
                               Result    = entities,
                               Exception = result.Exception,
                               Conflicts = result.Conflicts,
                               Content   = result.Content
                       };
            }
            catch (Exception e)
            {
                return new ContextCommitResult<IReadOnlyCollection<TEntity>>
                {
                               Exception = e
                       };
            }
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<TEntity>> UpdateAsync(TEntity entity)
        {
            try
            {
                var repository = _context.GetRepository<TEntity>();

                repository.Update(entity);

                var result = await _context.ExecuteCommitAsync();

                return new ContextCommitResult<TEntity>
                       {
                               Result    = entity,
                               Exception = result.Exception,
                               Conflicts = result.Conflicts,
                               Content   = result.Content
                       };
            }
            catch (Exception e)
            {
                return new ContextCommitResult<TEntity>
                       {
                               Exception = e
                       };
            }
        }

        /// <inheritdoc />
        public async Task<ContextCommitResult<IReadOnlyCollection<TEntity>>> UpdateManyAsync(params TEntity[] entities)
        {
            try
            {
                var repository = _context.GetRepository<TEntity>();

                repository.UpdateMany(entities);

                var result = await _context.ExecuteCommitAsync();

                return new ContextCommitResult<IReadOnlyCollection<TEntity>>
                       {
                               Result    = entities,
                               Exception = result.Exception,
                               Conflicts = result.Conflicts,
                               Content   = result.Content
                       };
            }
            catch (Exception e)
            {
                return new ContextCommitResult<IReadOnlyCollection<TEntity>>
                       {
                               Exception = e
                       };
            }
        }

        #region GetOne

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> entitySelector, CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetOneAsync(entitySelector, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TSelectEntity> GetOneAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, Expression<Func<TEntity, bool>> entityPredicate, CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetOneAsync(selector,  entityPredicate, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TEntity> GetOneAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetOneAsync(specification, cancellationToken);
        }

        public Task<TSelectEntity> GetOneAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> entitySelector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetOneAsync(entitySelector, specification, cancellationToken);
        }

        #endregion

        #region GetAll

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetAllAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector, CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetAllAsync(selector, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetAllAsync( specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetAllAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IOrderSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetAllAsync(selector, specification, cancellationToken);
        }

        #endregion

        #region GetMany

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> entitiesSelector,
                                                       Expression<Func<TEntity, object>> orderSelector,
                                                       bool isDescending,
                                                       CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetManyAsync(entitiesSelector, orderSelector, isDescending, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity>(Expression<Func<TEntity, TSelectEntity>> selector,
                                                                                  Expression<Func<TEntity, bool>> entitiesSelector,
                                                                                  Expression<Func<TEntity, object>> orderSelector,
                                                                                  bool isDescending,
                                                                                  CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetManyAsync(selector, entitiesSelector, orderSelector, isDescending, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetManyAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetManyAsync(  specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TSelectEntity>> GetManyAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IFilterSpecification<TEntity>, IOrderSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetManyAsync(selector, specification, cancellationToken);
        }

        #endregion
        
        #region GetPage

        public Task<PagedList<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, object>> order, bool isDescendingOrder, int pageSize, int pageIndex, CancellationToken cancellationToken = default)
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetPageAsync(criteria, order, isDescendingOrder, pageSize, pageIndex, cancellationToken);
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
            var repository = _context.GetRepository<TEntity>();

            return repository.GetPageAsync(selector, criteria, order, isDescendingOrder, pageSize, pageIndex, cancellationToken);
        }

        public Task<PagedList<TEntity>> GetPageAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetPageAsync(specification, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedList<TSelectEntity>> GetPageAsync<TSelectEntity, TSpecification>(Expression<Func<TEntity, TSelectEntity>> selector, TSpecification specification, CancellationToken cancellationToken = default)
                where TSpecification : IPaginationSpecification<TEntity>, IOrderSpecification<TEntity>, IFilterSpecification<TEntity>
        {
            var repository = _context.GetRepository<TEntity>();

            return repository.GetPageAsync(selector, specification, cancellationToken);
        }

        #endregion
    }
}