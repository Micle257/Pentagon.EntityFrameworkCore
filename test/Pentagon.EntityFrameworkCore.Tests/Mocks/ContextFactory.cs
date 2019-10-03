namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using Interfaces;

    public class ContextFactory : IContextFactory<IContext>
    {
        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null)
        {
            return new Context();
        }
    }

    public class NewContextFactory : IContextFactory<INewContext>
    {
        /// <inheritdoc />
        public IApplicationContext CreateContext(string[] args = null)
        {
            return new NewContext();
        }
    }
}