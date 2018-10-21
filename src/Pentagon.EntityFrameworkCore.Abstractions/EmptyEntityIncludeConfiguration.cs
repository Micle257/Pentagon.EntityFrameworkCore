// -----------------------------------------------------------------------
//  <copyright file="EmptyEntityIncludeConfiguration.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Specifications;

    public class EmptyEntityIncludeConfiguration : IEntityIncludeConfiguration
    {
        /// <inheritdoc />
        public void Configure<T>(ISpecification<T> specification)
                where T : IEntity { }
    }
}