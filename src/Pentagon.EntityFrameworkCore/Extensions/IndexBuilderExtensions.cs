// -----------------------------------------------------------------------
//  <copyright file="IndexBuilderExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Interfaces.Entities;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class IndexBuilderExtensions
    {
        [NotNull]
        public static IndexBuilder HasFilterForDeletedFlag([NotNull] this IndexBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            // only apply filter if entity type implements deleted flag support interface
            if (builder.Metadata.DeclaringEntityType.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeletedFlagSupport)))
                builder.HasFilter($"{nameof(IDeletedFlagSupport.DeletedFlag)} = 0");

            return builder;
        }
    }
}