// -----------------------------------------------------------------------
//  <copyright file="EntityTypeBuilderExtensions.cs">
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
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> SetupCreatedTimeSpanEntityDefaults<T>(this EntityTypeBuilder<T> builder)
                where T : class, ICreateTimeStampSupport
        {
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();
            
            return builder;
        }

        public static EntityTypeBuilder<T> SetupUpdatedTimeSpanEntityDefaults<T>(this EntityTypeBuilder<T> builder)
                where T : class, IUpdateTimeStampSupport
        {
            builder.Property(p => p.UpdatedAt)
                   .IsRequired(false);

            return builder;
        }

        public static EntityTypeBuilder SetupCreatedTimeSpanEntityDefaults(this EntityTypeBuilder builder, Type type)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreateTimeStampSupport)))
                throw new InvalidCastException($"The type ({type.Name}) doesn't implement {nameof(ICreateTimeStampSupport)}");

            builder.Property(nameof(ICreateTimeStampSupport.CreatedAt))
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }

        public static EntityTypeBuilder SetupUpdatedTimeSpanEntityDefaults(this EntityTypeBuilder builder, Type type)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IUpdateTimeStampSupport)))
                throw new InvalidCastException($"The type ({type.Name}) doesn't implement {nameof(IUpdateTimeStampSupport)}");
            
            builder.Property(nameof(IUpdateTimeStampSupport.UpdatedAt))
                   .IsRequired(false);

            return builder;
        }
    }
}