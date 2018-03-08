using System;
using Xunit;

namespace Pentagon.Data.EntityFramework.Tests
{
    using Abstractions;
    using Abstractions.Entities;
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
