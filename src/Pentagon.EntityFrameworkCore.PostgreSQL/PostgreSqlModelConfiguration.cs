// -----------------------------------------------------------------------
//  <copyright file="PostgreSqlModelConfiguration.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.PostgreSQL
{
    using Extensions;
    using Microsoft.EntityFrameworkCore;

    public class PostgreSqlModelConfiguration : ModelConfiguration
    {
        /// <inheritdoc />
        protected override string DefaultCreateGuidDatabaseFunction => "uuid_generate_v4()";

        /// <inheritdoc />
        protected override void SetupModelCore(ModelBuilder builder)
        {
            builder.HasPostgresExtension(name: "uuid-ossp");
        }
    }
}