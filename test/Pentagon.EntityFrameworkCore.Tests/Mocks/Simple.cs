namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using Abstractions.Entities;

    public class Simple : IEntity<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }

        /// <inheritdoc />
        object IEntity.Id
        {
            get { return Id; }
            set { Id = (int)value; }
        }
    }

    public class Person : EntityFrameworkCore.Entity
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}