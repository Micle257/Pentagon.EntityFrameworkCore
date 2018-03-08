namespace Pentagon.Data.EntityFramework.Abstractions {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Specifications;

    public interface IPaginationService
    {
        Task<IPagedList<TEntity>> CreateAsync<TEntity>(IQueryable<TEntity> query, IPaginationSpecification<TEntity> specification)
            where TEntity : IEntity;
    }
}