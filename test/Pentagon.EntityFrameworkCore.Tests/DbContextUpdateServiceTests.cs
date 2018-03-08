// -----------------------------------------------------------------------
//  <copyright file="DbContextUpdateServiceTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.Data.EntityFramework.Tests
{
    using System;
    using Xunit;

    public class DbContextUpdateServiceTests
    {
        [Fact]
        public void ShouldApplyWhenEntityIsAdded()
        {
            using (var db = new Context())
            {
                var entity = new Entity {Value = "ss"};
                db.Add(entity);
                var service = new DbContextUpdateService();
                service.Apply(db);

                Assert.Null(entity.LastUpdatedAt);
                Assert.Null(entity.DeletedAt);
                Assert.False(entity.IsDeletedFlag);
                Assert.NotEqual(default(DateTimeOffset),entity.CreatedAt);
                Assert.NotEqual(default(Guid), entity.ConcurrencyStamp);
                Assert.NotEqual(default(Guid), entity.CreateGuid);
            }
        }

        [Fact]
        public void ShouldNotApplyWhenEntityIsNotBaseEntity()
        {
            using (var db = new Context())
            {
                var entity = new Simple { Data = "Hi" };
                db.Add(entity);
                var service = new DbContextUpdateService();
                service.Apply(db);
            }
        }
    }
}