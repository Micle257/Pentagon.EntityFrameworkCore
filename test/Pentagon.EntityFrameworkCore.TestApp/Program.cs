namespace Pentagon.EntityFrameworkCore.TestApp
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Extensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;
    using PostgreSQL;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            var di = services.BuildServiceProvider();

            var userProvider = di.GetRequiredService<IDataUserProvider>();

            userProvider.UserId = 3;
            userProvider.UserName = "test";

            using (var c = di.GetRequiredService<Context>())
            {
                var j = c.Set<User>().AsTracking().ToList();

                j[0].Name = "FF";
                
                var r = c.ExecuteCommit();

                if (r.HasConcurrencyConflicts)
                {
                    foreach (var concurrencyConflictPair in r.Conflicts)
                    {
                        var d = concurrencyConflictPair.GetDifference();
                    }
                }
            }
        }
    }
}
