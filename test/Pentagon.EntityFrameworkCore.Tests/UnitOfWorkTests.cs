namespace Pentagon.EntityFrameworkCore.Tests {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EntityFrameworkCore;
    using Extensions;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Mocks;
    using Synchronization;
    using Threading;
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
                    .AddUnitOfWork<NewContext, INewContext, NewContextFactory>()
                    .AddDefaultAppContext<IContext>();

            var di = services.BuildServiceProvider();

            var factory = di.GetService<IContextFactory<IContext>>();
            var factory1 = di.GetService<IContextFactory>();
            var factory2 = di.GetService<IContextFactory<IApplicationContext>>();
            var factory3 = di.GetService<IContextFactory<INewContext>>();

            var com = di.GetService<IContext>();

            var unit = factory.CreateContext();

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
            com.ExecuteCommit();

            var entities = unit.GetRepository<Entity>().GetAllAsync().GetAwaiter().GetResult();

            var client = man.GetChange(null, entities).GetAwaiter().GetResult();
        }
        
        [Fact]
        public void FactMethodName()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            var di = services.BuildServiceProvider();

            var unitFactory = di.GetRequiredService<IContextFactory<IApplicationContext>>();
            var com = di.GetRequiredService<IApplicationContext>();
            var change = new DatabaseChangeManager<IApplicationContext>(unitFactory);

            var unit = unitFactory.CreateContext();

            var rep = unit.GetRepository<Entity>();

            rep.Insert(new Entity{Value = "1"});
            rep.Insert(new Entity{Value = "2"});

            com.ExecuteCommit();
            Task.Delay(1000);
            var time = DateTimeOffset.Now;

            rep.Insert(new Entity{Value = "3"});
            rep.Insert(new Entity{Value = "4"});
            var two = rep.GetOneAsync(a => a.Value == "2").AwaitSynchronously();
            two.Value = "2new";
            rep.Update(two);

            com.ExecuteCommit();
            Task.Delay(1000);

           var one = rep.GetOneAsync(a => a.Value == "1").AwaitSynchronously();
            rep.Delete(one);

            com.ExecuteCommit();
            Task.Delay(1000);
        }
    }
}