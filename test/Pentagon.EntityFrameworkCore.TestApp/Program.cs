using System;

namespace Pentagon.EntityFrameworkCore.TestApp
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Abstractions;
    using Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.DependencyInjection;
    using Repositories;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddUnitOfWork<Context, IApplicationContext>();

            var di = services.BuildServiceProvider();

            using (var c = di.GetRequiredService<Context>())
            {

            }
        }
    }

    class User : Entity, ICreateStampSupport
    {
        /// <inheritdoc />
        public Guid Uuid { get; set; }

        public string Name { get; set; }
    }

    class UserConfig : IEntityTypeConfiguration<User>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(256);
        }
    }

    class Context : BaseApplicationContext
    {
        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=PENTAGONDELL\\SQLEXPRESS;Initial Catalog=PentagonEFTest;Integrated Security=True");
        }

        /// <inheritdoc />
        protected override void OnModelCreatingCore(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
