namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using Abstractions;

    public class ContextFactory : IContextFactory<IContext>
    {
        /// <inheritdoc />
        public IContext CreateContext(string[] args = null)
        {
            return new Context();
        }
    }

    public class NewContextFactory : IContextFactory<INewContext>
    {
        /// <inheritdoc />
        public INewContext CreateContext(string[] args = null)
        {
            return new NewContext();
        }
    }
}