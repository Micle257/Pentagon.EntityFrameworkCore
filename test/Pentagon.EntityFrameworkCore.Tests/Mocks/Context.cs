namespace Pentagon.EntityFrameworkCore.Tests.Mocks
{
    using Abstractions;
    using EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class Context : DbContext, IApplicationContext
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
