// -----------------------------------------------------------------------
//  <copyright file="IndexBuilderExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using Abstractions.Entities;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public static class IndexBuilderExtensions
    {
        [NotNull]
        public static IndexBuilder HasIndexForDeletedFlag([NotNull] this IndexBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasFilter($"{nameof(IDeletedFlagSupport.DeletedFlag)} = 0");

            return builder;
        }
    }
}