// -----------------------------------------------------------------------
//  <copyright file="EntityIncludeConfiguration.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using Abstractions;
    using Abstractions.Entities;
    using JetBrains.Annotations;

    public class EntityIncludeConfiguration<TEntity> : IEntityIncludeConfiguration<TEntity>
            where TEntity : IEntity
    {
        [NotNull]
        readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _builder;

        public EntityIncludeConfiguration([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        /// <inheritdoc />
        public IQueryable<TEntity> Configure(IQueryable<TEntity> specification) => _builder(specification);
    }
}