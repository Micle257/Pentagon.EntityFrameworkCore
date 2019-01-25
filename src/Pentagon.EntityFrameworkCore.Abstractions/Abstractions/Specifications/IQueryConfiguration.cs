// -----------------------------------------------------------------------
//  <copyright file="IQueryConfiguration.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    
    public interface IQueryConfiguration<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the query configurations. </summary>
        /// <value> The query configurations. </value>
        List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> QueryConfigurations { get; }
    }
}