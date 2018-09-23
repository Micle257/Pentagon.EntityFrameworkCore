namespace Pentagon.EntityFrameworkCore.Tests.Mocks
{
    using Abstractions;
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public interface INewContext : IApplicationContext
    {

    }

    public interface IContext : IApplicationContext
    {

    }

    public class NewContext : DbContext, INewContext
    {
        public DbSet<Simple> Simples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("DB2");
            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        public bool HasHardDeleteBehavior { get; }
    }

    public class Context : DbContext, IContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("DB");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SetupBaseEntities();

            var entity = modelBuilder.Entity<Simple>();

            entity.Property(t => t.Id)
                  .ValueGeneratedOnAdd();

            entity.HasKey(t => t.Id);

            var entityEntity = modelBuilder.Entity<Entity>();
        }

        public DbSet<Simple> Simples { get; set; }

        public DbSet<Entity> Entities { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<TimestampIdentity> Identities { get; set; }

        public bool HasHardDeleteBehavior { get; }
    }
}