namespace Pentagon.EntityFrameworkCore.Abstractions {
    using Entities;
    using Specifications;

    public interface IEntityIncludeConfiguration
    {
        void Configure<T>(ISpecification<T> specification)
                where T : IEntity;
    }
}