namespace Pentagon.EntityFrameworkCore.Tests {
    using System;
    using System.Collections.Generic;
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
        public void ChangeTest()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IContext, ContextFactory>()
                    .AddDefaultAppContext<IContext>();

            var di = services.BuildServiceProvider();

            var factory = di.GetService<IUnitOfWorkFactory<IContext>>();
            var com = di.GetService<IUnitOfWorkCommitExecutor<IContext>>();

            var unit = factory.Create();

            var man = new DatabaseChangeManager<IContext>(factory);

            var clientPersons = new []
                                {
                                        new Entity
                                        {
                                                Value = "bis"
                                        },
                                        new Entity
                                        {
                                                Value = "lol"
                                        }
                                };

            unit.GetRepository<Entity>().InsertMany(clientPersons);
            com.ExecuteCommit(unit);

            var entities = unit.GetRepository<Entity>().GetAllAsync().Result;

            var client = man.GetChange(null, entities).Result;
        }

        [Fact]
        public void Test()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<NewContext, INewContext>()
                    .AddUnitOfWork<Context, IContext, ContextFactory>()
                    .AddDefaultAppContext<INewContext>();
            
            var di = services.BuildServiceProvider();

           var unit = di.GetService<IUnitOfWork>();

            var unit2 = di.GetService<IUnitOfWork<INewContext>>();

            var unit3 = di.GetService<IUnitOfWork<IApplicationContext>>();

            var unit5= di.GetService<IUnitOfWork<IContext>>();

            var ct = di.GetService<IApplicationContext>();
            var ct2 = di.GetService<Context>();
            var ct3 = di.GetService<NewContext>();
        }

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