namespace Pentagon.EntityFrameworkCore.TestApp {
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Options;

    class ContextFactory : IContextFactory<IApplicationContext> {
        readonly ILoggerFactory _logger;
        readonly IDbContextChangeService _deleteService;

        public ContextFactory(ILoggerFactory logger,
                              IDbContextChangeService deleteService)
        {
            _logger = logger;
            _deleteService = deleteService;
        }

        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null) => new Context(_logger.CreateLogger<Context>(), _deleteService,  new DbContextOptions<Context>()) {AutoResolveConflictsFromSameUser = true};
    }
}