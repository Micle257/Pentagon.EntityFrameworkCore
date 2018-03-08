// -----------------------------------------------------------------------
//  <copyright file="ModelBuilderExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.Data.EntityFramework
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Abstractions.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> SetupTimeSpanEntityDefaults<T>(this EntityTypeBuilder<T> builder)
            where T : class, ITimeStampSupport
        {
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                   .IsRequired();

            builder.Property(p => p.LastUpdatedAt)
                   .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }
        
        public static EntityTypeBuilder SetupTimeSpanEntityDefaults(this EntityTypeBuilder builder, Type type)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ITimeStampSupport)))
                throw new InvalidCastException($"The type ({type.Name}) doesn't implement {nameof(ITimeStampSupport)}");

            builder.Property(nameof(ITimeStampSupport.CreatedAt))
                   .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                   .IsRequired();

            builder.Property(nameof(ITimeStampSupport.LastUpdatedAt))
                   .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }
    }

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
                        .HasDefaultValueSql("NEWID()");

            return builder;
        }

        public static ModelBuilder SetupConcurrencyEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IConcurrencyStampSupport.ConcurrencyStamp))
                   .HasDefaultValueSql("NEWID()");

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
                   .HasDefaultValueSql("NEWID()");

            return builder;
        }

        public static ModelBuilder SetupCreateStampEntityDefaults(this ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(ICreateStampSupport.CreateGuid))
                   .HasDefaultValueSql("NEWID()");

            return builder;
        }
    }
}