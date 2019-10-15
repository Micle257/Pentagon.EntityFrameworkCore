namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Options;

    public class ContextFactory : IContextFactory<IContext>
    {
        readonly ILoggerFactory _loggerFactory;
        readonly IMemoryCache _cache;
        readonly IOptions<RepositoryCacheOptions> _repositoryOptions;
        readonly IDbContextChangeService _changeService;

        public ContextFactory(ILoggerFactory loggerFactory,
                              IMemoryCache cache,
                              IOptions<RepositoryCacheOptions> repositoryOptions, IDbContextChangeService changeService)
        {
            _loggerFactory = loggerFactory;
            _cache = cache;
            _repositoryOptions = repositoryOptions;
            _changeService = changeService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null)
        {
            return new Context(_loggerFactory.CreateLogger<Context>(), _changeService, _cache, _repositoryOptions, new DbContextOptions<Context>());
        }
    }

    public class NewContextFactory : IContextFactory<INewContext>
    {
        readonly ILoggerFactory _loggerFactory;
        readonly IMemoryCache _cache;
        readonly IOptions<RepositoryCacheOptions> _repositoryOptions;
        readonly IDbContextChangeService _changeService;

        public NewContextFactory(ILoggerFactory loggerFactory,
                                 IMemoryCache cache,
                                 IOptions<RepositoryCacheOptions> repositoryOptions, IDbContextChangeService changeService)
        {
            _loggerFactory = loggerFactory;
            _cache = cache;
            _repositoryOptions = repositoryOptions;
            _changeService = changeService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null)
        {
            return new NewContext(_loggerFactory.CreateLogger<NewContext>(), _changeService, _cache, _repositoryOptions, new DbContextOptions<NewContext>());
        }
    }
}