﻿// -----------------------------------------------------------------------
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

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreateTimeStampSupport)))
                    builder.SetupCreatedTimeSpanEntityDefaults(type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreatedUserEntitySupport)))
                    builder.SetupCreatedUserEntitySupportDefaults(type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IUpdateTimeStampSupport)))
                    builder.SetupUpdatedTimeSpanEntityDefaults(type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeletedFlagSupport)))
                    builder.SetupDeleteFlagEntityDefaults(type.ClrType);
            }

            return builder;
        }
        
        public static ModelBuilder SetupConcurrencyEntityDefaults<T>(this ModelBuilder builder)
                where T : class, IConcurrencyStampSupport
        {
            builder.Entity<T>()
                   .Property(p => p.ConcurrencyStamp)
                   .HasDefaultValueSql(sql: "NEWID()")
                   .IsRequired();

            return builder;
        }

        public static ModelBuilder SetupConcurrencyEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IConcurrencyStampSupport.ConcurrencyStamp))
                   .HasDefaultValueSql(sql: "NEWID()")
                   .IsRequired();

            return builder;
        }

        public static ModelBuilder SetupCreatedTimeSpanEntityDefaults<T>(this ModelBuilder builder)
                where T : class, ICreateTimeStampSupport
        {
            builder.Entity<T>().SetupCreatedTimeSpanEntityDefaults();

            return builder;
        }

        public static ModelBuilder SetupUpdatedTimeSpanEntityDefaults<T>(this ModelBuilder builder)
                where T : class, IUpdateTimeStampSupport
        {
            builder.Entity<T>().SetupUpdatedTimeSpanEntityDefaults();

            return builder;
        }

        public static ModelBuilder SetupCreatedTimeSpanEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type).SetupCreatedTimeSpanEntityDefaults(type);

            return builder;
        }

        public static ModelBuilder SetupCreatedUserEntitySupportDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type).Property(nameof(ICreatedUserEntitySupport.CreatedUser))
                   .HasDefaultValue("dbo")
                   .IsRequired();

            return builder;
        }

        public static ModelBuilder SetupUpdatedTimeSpanEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type).SetupUpdatedTimeSpanEntityDefaults(type);
            return builder;
        }

        public static ModelBuilder SetupCreateStampEntityDefaults<T>(this ModelBuilder builder)
                where T : class, ICreateStampSupport
        {
            builder.Entity<T>()
                   .Property(p => p.Uuid)
                   .HasDefaultValueSql(sql: "NEWID()");

            return builder;
        }

        public static ModelBuilder SetupCreateStampEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(ICreateStampSupport.Uuid))
                   .HasDefaultValueSql(sql: "NEWID()");

            return builder;
        }

        public static ModelBuilder SetupDeleteFlagEntityDefaults<T>(this ModelBuilder builder)
                where T : class, ICreateStampSupport => builder.SetupDeleteFlagEntityDefaults(typeof(T));

        public static ModelBuilder SetupDeleteFlagEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IDeletedFlagSupport.DeletedFlag))
                   .HasDefaultValue(0);

            return builder;
        }
    }
}