// -----------------------------------------------------------------------
//  <copyright file="ModelBuilderExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;

    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetupBaseEntities(this ModelBuilder builder)
        {
            foreach (var type in builder.Model.GetEntityTypes())
            {
                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreateStampSupport)))
                    builder.SetupCreateStampEntityDefaults(type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IConcurrencyStampSupport)))
                    builder.SetupConcurrencyEntityDefaults(type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ITimeStampSupport)))
                    builder.SetupTimeSpanEntityDefaults(type.ClrType);
            }

            return builder;
        }

        public static ModelBuilder SetupConcurrencyEntityDefaults<T>(this ModelBuilder builder)
                where T : class, IConcurrencyStampSupport
        {
            builder.Entity<T>()
                   .Property(p => p.ConcurrencyStamp)
                   .HasDefaultValueSql(sql: "NEWID()");

            return builder;
        }

        public static ModelBuilder SetupConcurrencyEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IConcurrencyStampSupport.ConcurrencyStamp))
                   .HasDefaultValueSql(sql: "NEWID()");

            return builder;
        }

        public static ModelBuilder SetupTimeSpanEntityDefaults<T>(this ModelBuilder builder)
                where T : class, ITimeStampSupport
        {
            builder.Entity<T>().SetupTimeSpanEntityDefaults();

            return builder;
        }

        public static ModelBuilder SetupTimeSpanEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type).SetupTimeSpanEntityDefaults(type);

            return builder;
        }

        public static ModelBuilder SetupCreateStampEntityDefaults<T>(this ModelBuilder builder)
                where T : class, ICreateStampSupport
        {
            builder.Entity<T>()
                   .Property(p => p.CreateGuid)
                   .HasDefaultValueSql(sql: "NEWID()");

            return builder;
        }

        public static ModelBuilder SetupCreateStampEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(ICreateStampSupport.CreateGuid))
                   .HasDefaultValueSql(sql: "NEWID()");

            return builder;
        }
    }
}