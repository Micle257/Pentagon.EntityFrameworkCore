using System;

namespace Pentagon.EntityFrameworkCore.TestApp
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Abstractions;
    using Abstractions.Entities;
    using Extensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using PostgreSQL;
    using Repositories;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            var di = services.BuildServiceProvider();

            using (var c = di.GetRequiredService<Context>())
            {
                var j = c.Set<User>().AsTracking().ToList();

                j[0].Name = "FF";
                
                var r = c.ExecuteCommit();

                if (r.HasConcurrencyConflicts)
                {
                    foreach (var concurrencyConflictPair in r.Conflicts)
                    {
                        var d = concurrencyConflictPair.GetDifference();
                    }
                }
            }
        }
    }

    class ContextFactory : IContextFactory<IApplicationContext> {
        readonly ILoggerFactory _logger;
        readonly IDbContextUpdateService _updateService;
        readonly IDbContextDeleteService _deleteService;

        public ContextFactory(ILoggerFactory logger,
                              IDbContextUpdateService updateService,
                              IDbContextDeleteService deleteService)
        {
            _logger = logger;
            _updateService = updateService;
            _deleteService = deleteService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null) => new Context(_logger.CreateLogger<Context>(), _updateService, _deleteService, new DbContextOptions<Context>());
    }

    class User : Entity, ICreateStampSupport, IConcurrencyStampSupport
    {
        /// <inheritdoc />
        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public int? a1 { get; set; }

        public double? a2 { get; set; }

        public char a3 { get; set; }

        /// <inheritdoc />
        public Guid ConcurrencyStamp { get; set; }
    }

    class UserConfig : IEntityTypeConfiguration<User>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(a => a.Name)
                   .IsUnique();

            builder.Property(a => a.a1)
                   .HasDefaultValue(6);
        }
    }

    class Context : BaseApplicationContext
    {
        public Context()
        {
            
        }

        public Context(ILogger<Context> logger,
                       IDbContextUpdateService updateService,
                       IDbContextDeleteService deleteService,
                       DbContextOptions<Context> options) : base(logger, updateService, deleteService, options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Data Source=PENTAGONDELL\\SQLEXPRESS;Initial Catalog=PentagonEFTest;Integrated Security=True");
            //.UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw");
        }
        
        /// <inheritdoc />
        protected override void OnModelCreatingCore(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
