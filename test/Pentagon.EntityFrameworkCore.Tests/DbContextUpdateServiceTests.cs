// -----------------------------------------------------------------------
//  <copyright file="DbContextUpdateServiceTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.EntityFrameworkCore.Tests
{
    using System;
    using EntityFrameworkCore;
    using Extensions;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Mocks;
    using Xunit;
    using Entity = Mocks.Entity;

    public class DbContextUpdateServiceTests
    {
        public DbContextUpdateServiceTests()
        {
            var id = Guid.NewGuid();

            var services = new ServiceCollection()
                    .AddLogging();

            services.AddUnitOfWork<Context, IApplicationContext, ContextFactory>();

            DI = services.BuildServiceProvider();
        }

        IServiceProvider DI;

        [Fact]
        public void ShouldApplyWhenEntityIsAdded()
        {
            var unit = DI.GetService<IApplicationContext>();
            var service = DI.GetService<IDbContextChangeService>();
            var user = DI.GetService<IDataUserIdentityWriter>();

            user.SetIdentity(2, "bis");

            var db = unit.GetRepository<Entity>();

            var entity = new Entity { Value = "ss" };

            db.Insert(entity);

            service.ApplyUpdate(unit);
            service.ApplyConcurrency(unit);

            Assert.Null(entity.UpdatedAt);
            Assert.Null(entity.DeletedAt);
            Assert.Null(entity.UpdatedUserId);
            Assert.Null(entity.DeletedUserId);
            Assert.False(entity.DeletedFlag);
            
            Assert.NotNull(entity.CreatedUserId);
            Assert.Equal("bis", entity.CreatedUser);
            Assert.NotEqual(default(DateTimeOffset), entity.CreatedAt);
            Assert.NotEqual(default(Guid), entity.ConcurrencyStamp);
            Assert.NotEqual(default(Guid), entity.Uuid);
        }

        [Fact]
        public void ShouldApplyWhenEntityIsModified()
        {
            var service = DI.GetService<IDbContextChangeService>();
            var ex = DI.GetService<IApplicationContext>();
            var unit = DI.GetService<IContextFactory>().CreateContext( );
            var user = DI.GetService<IDataUserIdentityWriter>();

            user.SetIdentity(2, null);

            var db = unit.GetRepository<Entity>();

            var entity = new Entity { Value = "ss" };

            db.Insert(entity);

            ex.ExecuteCommit();

            var e = db.GetOneAsync(a => true).GetAwaiter().GetResult();
            e.Value = "qwe";

            db.Update(e);

            service.ApplyUpdate(unit);
            service.ApplyConcurrency(unit);

            Assert.NotNull(entity.UpdatedAt);
            Assert.Null(entity.DeletedAt);
            Assert.NotNull(entity.UpdatedUserId);
            Assert.Null(entity.DeletedUserId);
            Assert.False(entity.DeletedFlag);
            Assert.Null(entity.CreatedUser);
        }
    }
}