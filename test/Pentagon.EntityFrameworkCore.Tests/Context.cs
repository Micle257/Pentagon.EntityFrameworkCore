using System;
using Xunit;

namespace Pentagon.Data.EntityFramework.Tests
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using EntityFrameworkCore;
    using EntityFrameworkCore.Abstractions;
    using EntityFrameworkCore.Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class UnitOfWorkTests
    {
        [Fact]
        public void Test()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            var di = services.BuildServiceProvider();

            var sc = di.GetRequiredService<IUnitOfWorkScope<IApplicationContext>>();
            var sc2 = di.GetRequiredService<IUnitOfWorkScope<IApplicationContext>>();
            var com = di.GetRequiredService<IDatabaseCommitManager>();

            using (sc)
            {
                var db = sc.Get();
                var repo = db.GetRepository<Entity>();

                // add sample data
                repo.InsertMany(new Entity { Value = "first" },
                                new Entity { Value = "second" },
                                new Entity { Value = "last" });
            }

            using (sc)
            {
                var db = sc.Get();
                var repo = db.GetRepository<Entity>();

                var data = repo.GetAllAsync().Result.ToList();

                data[0].Value = "new first";
                repo.Update(data[0]);

                using (sc2)
                {
                    var db2 = sc2.Get();
                    var repo2 = db2.GetRepository<Entity>();

                    var data2 = repo2.GetAllAsync().Result.ToList();

                    data2[0].Value = "new new first";
                    repo2.Update(data2[0]);
                }
            }

            using (sc)
            {
                var db = sc.Get();
                var repo = db.GetRepository<Entity>();

                var data = repo.GetAllAsync().Result.ToList();
            }
        }
    }

    public class ContextFactory : IContextFactory<IApplicationContext>
    {
        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null)
        {
            return new Context();
        }
    }

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

        public DbSet<TimestampIdentity> Identities { get; set; }

        public bool HasHardDeleteBehavior { get; }
    }

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

    public class Entity : SyncEntity<int>
    {
        public string Value { get; set; }
    }

    public class TimestampIdentity : TimestampIdentityEntity<Guid>
    {
        public string Value { get; set; }
    }
}
