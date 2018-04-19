namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications {
    using System;
    using System.Linq.Expressions;
    using Entities;

    /// <summary> Represents a entity specification for query pipeline, that is capable of specifing what entities should be returned. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface ICriteriaSpecification<TEntity> : ISpecification<TEntity>
        where TEntity : IEntity
    {
        /// <summary> Gets the criteria function for specification. </summary>
        /// <value> The expression tree of the criteria. </value>
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}