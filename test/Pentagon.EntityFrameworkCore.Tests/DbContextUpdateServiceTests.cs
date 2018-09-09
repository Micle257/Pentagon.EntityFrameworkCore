// -----------------------------------------------------------------------
//  <copyright file="DbContextUpdateServiceTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.EntityFrameworkCore.Tests
{
    using System;
    using Abstractions;
    using EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Mocks;
    using Xunit;
    using Entity = Mocks.Entity;

    public class DbContextUpdateServiceTests
    {
        public DbContextUpdateServiceTests()
        {
            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            DI = services.BuildServiceProvider();
        }

        IServiceProvider DI;

        [Fact]
        public void ShouldApplyWhenEntityIsAdded()
        {
            var unit = DI.GetService<IUnitOfWork<IApplicationContext>>();
            var service = DI.GetService<IDbContextUpdateService>();
            var user = DI.GetService<IDataUserProvider>();

            user.UserId = 2;

            var db = unit.GetRepository<Entity>();

            var entity = new Entity { Value = "ss" };

            db.Insert(entity);
            
            service.Apply(unit, DateTimeOffset.Now);

            Assert.Null(entity.UpdatedAt);
            Assert.Null(entity.DeletedAt);
            Assert.Null(entity.UpdatedBy);
            Assert.Null(entity.DeletedBy);
            Assert.False(entity.IsDeletedFlag);
            
            Assert.NotNull(entity.CreatedBy);
            Assert.NotEqual(default(DateTimeOffset), entity.CreatedAt);
            Assert.NotEqual(default(Guid), entity.ConcurrencyStamp);
            Assert.NotEqual(default(Guid), entity.Uuid);
        }

        [Fact]
        public void ShouldApplyWhenEntityIsModified()
        {
            var ex = DI.GetService<IUnitOfWorkCommitExecutor<IApplicationContext>>();
            var unit = DI.GetService<IUnitOfWork<IApplicationContext>>();
            var service = DI.GetService<IDbContextUpdateService>();
            var user = DI.GetService<IDataUserProvider>();

            user.UserId = 2;

            var db = unit.GetRepository<Entity>();

            var entity = new Entity { Value = "ss" };

            db.Insert(entity);

            ex.ExecuteCommit(unit);

            var e = db.GetOneAsync(a => true).Result;
            e.Value = "qwe";

            db.Update(e);
            
            service.Apply(unit, DateTimeOffset.Now);

            Assert.NotNull(entity.UpdatedAt);
            Assert.Null(entity.DeletedAt);
            Assert.NotNull(entity.UpdatedBy);
            Assert.Null(entity.DeletedBy);
            Assert.False(entity.IsDeletedFlag);
        }
    }
}