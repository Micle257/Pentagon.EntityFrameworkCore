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
        readonly IDbContextChangeService _changeService;

        public ContextFactory(ILoggerFactory loggerFactory, IDbContextChangeService changeService)
        {
            _loggerFactory = loggerFactory;
            _changeService = changeService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext()
        {
            return new Context(_loggerFactory.CreateLogger<Context>(), _changeService, new DbContextOptions<Context>());
        }
    }

    public class NewContextFactory : IContextFactory<INewContext>
    {
        readonly ILoggerFactory _loggerFactory;
        readonly IDbContextChangeService _changeService;

        public NewContextFactory(ILoggerFactory loggerFactory,IDbContextChangeService changeService)
        {
            _loggerFactory = loggerFactory;
            _changeService = changeService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext()
        {
            return new NewContext(_loggerFactory.CreateLogger<NewContext>(), _changeService,  new DbContextOptions<NewContext>());
        }
    }
}