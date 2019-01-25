namespace Pentagon.EntityFrameworkCore.Abstractions {
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Entities;
    using Specifications;

    public interface IEntityIncludeConfiguration<TEntity>
            where TEntity : IEntity
    {
        IQueryable<TEntity> Configure(IQueryable<TEntity> specification);
    }
}