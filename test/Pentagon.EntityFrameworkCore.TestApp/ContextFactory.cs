namespace Pentagon.EntityFrameworkCore.TestApp {
    using Abstractions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

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
        public IApplicationContext CreateContext(string[] args = null) => new Context(_logger.CreateLogger<Context>(), _updateService, _deleteService, new DbContextOptions<Context>()) {AutoResolveConflictsFromSameUser = true};
    }
}