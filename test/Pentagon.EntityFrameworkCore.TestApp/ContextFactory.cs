namespace Pentagon.EntityFrameworkCore.TestApp {
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Options;

    class ContextFactory : IContextFactory<IApplicationContext> {
        readonly ILoggerFactory _logger;
        readonly IMemoryCache _cache;
        readonly IOptions<RepositoryCacheOptions> _repositoryOptions;
        readonly IDbContextChangeService _deleteService;

        public ContextFactory(ILoggerFactory logger,
                              IMemoryCache cache,
                              IOptions<RepositoryCacheOptions> repositoryOptions,
                              IDbContextChangeService deleteService)
        {
            _logger = logger;
            _cache = cache;
            _repositoryOptions = repositoryOptions;
            _deleteService = deleteService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null) => new Context(_logger.CreateLogger<Context>(), _deleteService, _cache, _repositoryOptions, new DbContextOptions<Context>()) {AutoResolveConflictsFromSameUser = true};
    }
}