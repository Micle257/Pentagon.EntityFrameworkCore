namespace Pentagon.EntityFrameworkCore.TestApp
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Extensions;
    using Filters;
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

            var userProvider = di.GetRequiredService<IDataUserIdentityWriter>();

            userProvider.SetIdentity(3, "test");

            using (var c = di.GetRequiredService<Context>())
            {
                var filter = new FilterBuilder<User>()
                       .AddCompositeFilter(a => a.Name, TextFilter.Contain, "2")
                            .AddSubFilter(FilterLogicOperation.Or, TextFilter.Contain, "TgT")
                            .BuildFilter();

                var j = c.Set<User>().Where(filter).ToList();
            }

            var builder = new FilterBuilder<User>();

            builder.AddCompositeFilter(a => a.CreatedAt, d => d.Day == 3)
                   .AddSubFilter(FilterLogicOperation.Or, d => d.Day > 8);

            builder.AddCompositeFilter(a => a.Name == "we")
                   .AddSubFilter(FilterLogicOperation.And, c => c.UpdatedAt.HasValue);

            builder.AddCompositeFilter(c => c.Name, TextFilter.EndWith, "po")
                   .AddSubFilter(FilterLogicOperation.And, TextFilter.Contain, "aw");
        }
    }
}
