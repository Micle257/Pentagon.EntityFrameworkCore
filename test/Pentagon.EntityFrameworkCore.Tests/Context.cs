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

            var unit = di.GetRequiredService<IUnitOfWork<IApplicationContext>>();
            var com = di.GetRequiredService<IDatabaseCommitManager>();

            com.Commiting += (sender, args) =>
                             {

                             };

            com.Commited += (sender, args) =>
                            {

                            };

            var simpleRepo = unit.GetRepository<Simple>();

            // add 
            simpleRepo.Insert(new Simple { Data = "lol", Id = 1 });
            simpleRepo.Insert(new Simple { Data = "more", Id = 2 });

            unit.Commit();

            //get 
            var result = simpleRepo.GetAllAsync().Result.ToList();

            // update
            result[0].Data = "MO";
            simpleRepo.Update(result[0]);
            simpleRepo.Update(result[1]);

            // delete
            simpleRepo.Delete(result[1]);
            unit.Commit();

            ///////////////
            using (var db = di.GetService<IUnitOfWorkScope<IApplicationContext>>().Get())
            {
                var r = db.GetRepository<Simple>();

                var items = r.GetAllAsync().Result.ToList();

                items[0].Data = "qweqwewqeqwe";
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

            var entity = modelBuilder.Entity<Simple>();

            entity
                         .Property(t => t.Id)
                         .ValueGeneratedOnAdd();

            entity.HasKey(t => t.Id);
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
