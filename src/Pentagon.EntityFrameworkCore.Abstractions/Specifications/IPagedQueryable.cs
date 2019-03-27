// -----------------------------------------------------------------------
//  <copyright file="IPagedQueryable.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System.Linq;

    public interface IPagedQueryable<out T> : IQueryable<T>
    {
        PaginationParameters PaginationParameters { get; }
    }
}