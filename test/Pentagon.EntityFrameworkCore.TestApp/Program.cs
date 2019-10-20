namespace Pentagon.EntityFrameworkCore.TestApp
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Threading;
    using Extensions;
    using Filters;
    using Interfaces;
    using Interfaces.Stores;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;
    using Options;
    using PostgreSQL;
    using Repositories;
    using Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>()
                    .Configure<StoreCacheOptions>(o =>
                                                  {
                                                      o.Default.SlidingExpiration = TimeSpan.FromMinutes(1);
                                                      o.Entities.Add("User", new EntityCacheOptions
                                                                             {
                                                                                     SlidingExpiration = TimeSpan.FromSeconds(1),
                                                                                     AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                                                                             });
                                                  })
                    .AddStoreCached<User>(options =>
                                          {
                                              options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                                              options.SlidingExpiration = TimeSpan.FromSeconds(1);
                                          });

            var di = services.BuildServiceProvider();

            var store = di.GetRequiredService<IStore<User>>();

            var userProvider = di.GetRequiredService<IDataUserIdentityWriter>();

            userProvider.SetIdentity(3, "test");

                var filter = new FilterBuilder<User>()
                       .AddCompositeFilter(a => a.Name, TextFilter.Contain, "2")
                            .AddSubFilter(FilterLogicOperation.Or, TextFilter.Contain, "TgT")
                            .BuildFilter();

                var j = store.GetManyAsync(filter, a=>a.Id, false).AwaitSynchronously();

                store.InsertAsync(new User
                                  {
                                          Name = Guid.NewGuid().ToString()
                                  }).AwaitSynchronously();

            var builder = new FilterBuilder<User>();

            builder.AddCompositeFilter(a => a.CreatedAt.Day == 3)
                   .AddSubFilter(FilterLogicOperation.Or, d => d.CreatedAt.Day > 8);

            builder.AddCompositeFilter(a => a.Name == "we")
                   .AddSubFilter(FilterLogicOperation.And, c => c.UpdatedAt.HasValue);

            builder.AddCompositeFilter(c => c.Name, TextFilter.EndWith, "po")
                   .AddSubFilter(FilterLogicOperation.And, TextFilter.Contain, "aw");

            //store.GetManyAsync(filter, a => a.Id, false).AwaitSynchronously();
        }
    }
}
