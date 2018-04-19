namespace Pentagon.EntityFrameworkCore.Abstractions {
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Entities;
    using Specifications;

    public interface IPaginationService
    {
        Task<IPagedList<TSelectEntity>> CreateAsync<TSelectEntity, TEntity>(Expression<Func<TEntity, TSelectEntity>> selector, IQueryable<TEntity> query, IPaginationSpecification<TEntity> specification)
            where TEntity : IEntity;
    }
}