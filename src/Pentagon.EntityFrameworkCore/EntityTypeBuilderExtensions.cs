﻿// -----------------------------------------------------------------------
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
                where T : class, ICreatedTimeStampSupport
        {
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();
            
            return builder;
        }

        public static EntityTypeBuilder<T> SetupUpdatedTimeSpanEntityDefaults<T>(this EntityTypeBuilder<T> builder)
                where T : class, IUpdatedTimeStampSupport
        {
            builder.Property(p => p.LastUpdatedAt)
                   .IsRequired(false);

            return builder;
        }

        public static EntityTypeBuilder SetupCreatedTimeSpanEntityDefaults(this EntityTypeBuilder builder, Type type)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreatedTimeStampSupport)))
                throw new InvalidCastException($"The type ({type.Name}) doesn't implement {nameof(ICreatedTimeStampSupport)}");

            builder.Property(nameof(ICreatedTimeStampSupport.CreatedAt))
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }

        public static EntityTypeBuilder SetupUpdatedTimeSpanEntityDefaults(this EntityTypeBuilder builder, Type type)
        {
            if (!type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IUpdatedTimeStampSupport)))
                throw new InvalidCastException($"The type ({type.Name}) doesn't implement {nameof(IUpdatedTimeStampSupport)}");
            
            builder.Property(nameof(IUpdatedTimeStampSupport.LastUpdatedAt))
                   .IsRequired(false);

            return builder;
        }
    }
}