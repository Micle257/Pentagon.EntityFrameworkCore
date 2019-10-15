namespace Pentagon.EntityFrameworkCore.TestApp {
    using System.Reflection;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Options;
    using Repositories;

    class Context : BaseApplicationContext
    {
        public Context()
        {
            
        }

        public Context(ILogger<Context> logger,
                       IDbContextChangeService deleteService,
                       IMemoryCache cache,
                       IOptions<RepositoryCacheOptions> repositoryOptions,
                       DbContextOptions<Context> options) : base(logger, deleteService, cache, repositoryOptions, options)
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