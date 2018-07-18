// -----------------------------------------------------------------------
//  <copyright file="DbContextUpdateServiceTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.Data.EntityFramework.Tests
{
    using System;
    using System.Collections.Generic;
    using EntityFrameworkCore;
    using EntityFrameworkCore.Abstractions.Entities;
    using Xunit;

    public class DbContextIdentityServiceTests
    {
        [Fact]
        public void ShouldApplyWhenEntityIsAdded()
        {
            using (var db = new Context())
            {
                var entity = new TimestampIdentity();
                db.Add(entity);
                var service = new DbContextIdentityService();

                var user = Guid.NewGuid();
                var dict = new Dictionary<IEntity, object>
                           {
                             {entity, user }
                           };

                service.Apply(db, dict);

                Assert.Equal(user, entity.CreatedBy);
                Assert.Null(entity.UpdatedBy);
                Assert.Null(entity.DeletedBy);
            }
        }

        [Fact]
        public void ShouldApplyWhenEntityIsUpdated()
        {
            using (var db = new Context())
            {
                var entity = new TimestampIdentity();
                db.Add(entity);
                db.SaveChanges();
                entity.Value = "s";
                db.Update(entity);
                var service = new DbContextIdentityService();

                var user = Guid.NewGuid();
                var dict = new Dictionary<IEntity, object>
                           {
                               {entity, user }
                           };

                service.Apply(db, dict);

                Assert.NotEqual(user, entity.CreatedBy);
                Assert.Equal(user, entity.UpdatedBy);
                Assert.Null(entity.DeletedBy);
            }
        }

        [Fact]
        public void ShouldApplyWhenEntityIsDeleted()
        {
            using (var db = new Context())
            {
                var entity = new TimestampIdentity();
                db.Remove(entity);
                var service = new DbContextIdentityService();

                var user = Guid.NewGuid();
                var dict = new Dictionary<IEntity, object>
                           {
                               {entity, user }
                           };

                service.Apply(db, dict);

                Assert.NotEqual(user, entity.CreatedBy);
                Assert.Null(entity.UpdatedBy);
                Assert.Equal(user,entity.DeletedBy);
            }
        }
    }
}