// -----------------------------------------------------------------------
//  <copyright file="ApplicationContextExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Extensions
{
    using System;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public static class ApplicationContextExtensions
    {
        public static DbContext GetDbContext(this IApplicationContext context)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(context is DbContext dbContext))
                throw new ArgumentNullException(nameof(dbContext));

            return dbContext;
        }
    }
}