// -----------------------------------------------------------------------
//  <copyright file="Extensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    /// <summary>
    /// Contains extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary> Determines if given <see cref="DatabaseFacade" /> has difference in migration of assembly and database migrations. </summary>
        /// <param name="db"> The database. </param>
        /// <returns> A <see cref="Task" /> that represents the asynchronous operation. </returns>
        public static async Task<bool> HasMigrationDifferenceAsync([NotNull] this DatabaseFacade db)
        {
            var assemblyMigrations = db.GetMigrations().ToList();
            await db.EnsureCreatedAsync().ConfigureAwait(false);
            var databaseMigrations = (await db.GetAppliedMigrationsAsync().ConfigureAwait(false)).ToList();
            if (assemblyMigrations.Count == databaseMigrations.Count)
            {
                var t = assemblyMigrations.Zip(databaseMigrations, (s, s1) => assemblyMigrations.Any(s2 => s1 == s2));
                var enumerable = t as IList<bool> ?? t.ToList();
                if (enumerable.Count == 0)
                    return true;
                var r = enumerable.Aggregate((a, b) => a & b);
                if (!r)
                    return true;
            }
            else
                return true;

            return false;
        }

        /// <summary>
        /// Gets the name of the field name from property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The filed name.</returns>
        public static string GetFieldNameFromPropertyName(this string propertyName)
        {
            return "_" + propertyName.Select((c, i) => i == 0 ? c.ToString().ToLower() : c.ToString()).Aggregate((a, b) => $"{a}{b}");
        }
    }
}