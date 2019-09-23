namespace Pentagon.EntityFrameworkCore.TestApp {
    using Abstractions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

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
        public IApplicationContext CreateContext(string[] args = null) => new Context(_logger.CreateLogger<Context>(), _deleteService, new DbContextOptions<Context>()) {AutoResolveConflictsFromSameUser = true};
    }
}