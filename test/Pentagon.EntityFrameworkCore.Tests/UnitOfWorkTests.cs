namespace Pentagon.EntityFrameworkCore.Tests {
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Mocks;
    using Synchronization;
    using Xunit;
    using Entity = Mocks.Entity;

    public class UnitOfWorkTests
    {
        [Fact]
        public void FactMethodName()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            var di = services.BuildServiceProvider();

            var unitFactory = di.GetRequiredService<IUnitOfWorkFactory<IApplicationContext>>();
            var com = di.GetRequiredService<IUnitOfWorkCommitExecutor<IApplicationContext>>();
            var change = new DatabaseChangeManager<IApplicationContext>(unitFactory);

            var unit = unitFactory.Create();

            var rep = unit.GetRepository<Entity>();

            rep.Insert(new Entity{Value = "1"});
            rep.Insert(new Entity{Value = "2"});

            com.ExecuteCommit(unit);
            Task.Delay(1000);
            var time = DateTimeOffset.Now;

            rep.Insert(new Entity{Value = "3"});
            rep.Insert(new Entity{Value = "4"});
            var two = rep.GetOneAsync(a => a.Value == "2").Result;
            two.Value = "2new";
            rep.Update(two);

            com.ExecuteCommit(unit);
            Task.Delay(1000);

           var one = rep.GetOneAsync(a => a.Value == "1").Result;
            rep.Delete(one);

            com.ExecuteCommit(unit);
            Task.Delay(1000);

            var c= change.GetChange<Entity>(time).Result;
        }

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