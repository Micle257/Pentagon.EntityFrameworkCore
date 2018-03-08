namespace Pentagon.Data.EntityFramework.Abstractions.Specifications {
    using System.Linq;
    using Entities;

    /// <summary> Represents an entity specification for query pipeline, that is capable of pagination. </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IPaginationSpecification<TEntity> : IAllPaginationSpecification<TEntity>
        where TEntity : IEntity
    {
        /// <summary> Gets the index of the page. </summary>
        /// <value> The <see cref="int" />. </value>
        int PageIndex { get; set; }

        /// <summary> Applies the pagination specification to the query. </summary>
        /// <param name="query"> The query. </param>
        /// <returns> A modified query. </returns>
        IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query);
    }
}