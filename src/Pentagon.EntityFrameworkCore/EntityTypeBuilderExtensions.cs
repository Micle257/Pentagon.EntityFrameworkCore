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
        public static EntityTypeBuilder<T> SetupTimeSpanEntityDefaults<T>(this EntityTypeBuilder<T> builder)
                where T : class, ITimeStampSupport
        {
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            builder.Property(p => p.LastUpdatedAt)
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }

        public static EntityTypeBuilder SetupTimeSpanEntityDefaults(this EntityTypeBuilder builder, Type type)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ITimeStampSupport)))
                throw new InvalidCastException($"The type ({type.Name}) doesn't implement {nameof(ITimeStampSupport)}");

            builder.Property(nameof(ITimeStampSupport.CreatedAt))
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            builder.Property(nameof(ITimeStampSupport.LastUpdatedAt))
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }
    }
}