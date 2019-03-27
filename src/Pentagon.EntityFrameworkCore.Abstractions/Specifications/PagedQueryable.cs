// -----------------------------------------------------------------------
//  <copyright file="PagedQueryable.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.EntityFrameworkCore.Specifications {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    public class PagedQueryable<T> : IPagedQueryable<T>
    {
        [NotNull]
        readonly IQueryable<T> _innerQueryable;

        public PagedQueryable([NotNull] IQueryable<T> innerQueryable, PaginationParameters parameters)
        {
            _innerQueryable = innerQueryable ?? throw new ArgumentNullException(nameof(innerQueryable));

            PaginationParameters = parameters;
        }

        /// <inheritdoc />
        public PaginationParameters PaginationParameters { get;  }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _innerQueryable.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public Type ElementType => _innerQueryable.ElementType;

        /// <inheritdoc />
        public Expression Expression => _innerQueryable.Expression;

        /// <inheritdoc />
        public IQueryProvider Provider => _innerQueryable.Provider;
    }
}