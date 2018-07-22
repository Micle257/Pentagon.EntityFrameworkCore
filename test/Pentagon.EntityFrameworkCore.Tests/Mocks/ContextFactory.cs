namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using Abstractions;

    public class ContextFactory : IContextFactory<IApplicationContext>
    {
        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null)
        {
            return new Context();
        }
    }
}