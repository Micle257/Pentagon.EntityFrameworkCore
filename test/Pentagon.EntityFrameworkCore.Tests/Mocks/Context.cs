namespace Pentagon.EntityFrameworkCore.Tests.Mocks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using EntityFrameworkCore;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public interface INewContext : IApplicationContext
    {

    }

    public interface IContext : IApplicationContext
    {

    }

    public class NewContext : BaseApplicationContext, INewContext
    {
        public NewContext([NotNull] ILogger logger,
                       [NotNull] IDbContextChangeService deleteService,
                       DbContextOptions options) : base(logger, deleteService, options)
        {

        }

        public DbSet<Simple> Simples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class Context : BaseApplicationContext, IContext
    {
        public Context()
        {
            
        }

        public Context([NotNull] ILogger logger,
                       [NotNull] IDbContextChangeService deleteService,
                       DbContextOptions options) : base(logger, deleteService, options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreatingCore(ModelBuilder modelBuilder)
        {
            base.OnModelCreatingCore(modelBuilder);
            
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
    }
}
