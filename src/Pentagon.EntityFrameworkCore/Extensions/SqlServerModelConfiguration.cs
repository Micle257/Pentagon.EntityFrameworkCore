// -----------------------------------------------------------------------
//  <copyright file="ModelBuilderExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Extensions
{
    public class SqlServerModelConfiguration : ModelConfiguration
    {
        /// <inheritdoc />
        protected override string DefaultCreateGuidDatabaseFunction => "NEWID()";
    }
}