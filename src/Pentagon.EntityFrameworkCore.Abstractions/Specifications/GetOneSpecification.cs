namespace Pentagon.Data.EntityFramework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using JetBrains.Annotations;

    /// <summary> Represents a implementation of <see cref="ISpecification{TEntity}" /> for get one operations. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public class GetOneSpecification<TEntity> : ICriteriaSpecification<TEntity>
        where TEntity : IEntity
    {
        /// <inheritdoc />
        public  IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            if (Criteria != null)
                query = query.Where(Criteria);

            return query;
        }

        /// <inheritdoc />
        public IList<Expression<Func<TEntity, object>>> Includes { get; } = new List<Expression<Func<TEntity, object>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOneSpecification{TEntity}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public GetOneSpecification([NotNull] Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
        }

        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> Criteria { get; }
    }
}