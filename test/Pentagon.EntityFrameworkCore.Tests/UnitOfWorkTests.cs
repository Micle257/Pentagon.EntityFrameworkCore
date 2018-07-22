namespace Pentagon.EntityFrameworkCore.Tests {
    using System.Linq;
    using Abstractions;
    using EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Mocks;
    using Xunit;
    using Entity = Mocks.Entity;

    public class UnitOfWorkTests
    {
        [Fact]
        public void ConcurrencyTest()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            var di = services.BuildServiceProvider();

            var sc = di.GetRequiredService<IUnitOfWorkScope<IApplicationContext>>();
            var sc2 = di.GetRequiredService<IUnitOfWorkScope<IApplicationContext>>();
            var com = di.GetRequiredService<IDatabaseCommitManager>();

            using (sc)
            {
                var db = sc.Get();
                var repo = db.GetRepository<Entity>();

                // add sample data
                repo.InsertMany(new Entity { Value = "first" },
                                new Entity { Value = "second" },
                                new Entity { Value = "last" });
            }

            try
            {
                using (sc)
                {
                    var db = sc.Get();
                    var repo = db.GetRepository<Entity>();

                    var data = repo.GetAllAsync().Result.ToList();

                    data[0].Value = "new first";
                    repo.Update(data[0]);

                    using (sc2)
                    {
                        var db2 = sc2.Get();
                        var repo2 = db2.GetRepository<Entity>();

                        var data2 = repo2.GetAllAsync().Result.ToList();

                        data2[0].Value = "new new first";
                        repo2.Update(data2[0]);
                    }

                }
            }
            catch (UnitOfWorkConcurrencyConflictException e)
            {

            }

            using (sc)
            {
                var db = sc.Get();
                var repo = db.GetRepository<Entity>();

                var data = repo.GetAllAsync().Result.ToList();
            }
        }
    }
}