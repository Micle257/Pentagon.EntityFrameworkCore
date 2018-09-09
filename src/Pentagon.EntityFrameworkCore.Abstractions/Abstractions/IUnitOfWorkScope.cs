// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWorkScope.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;

    public interface IUnitOfWorkScope<out TContext> : IUnitOfWorkScope
            where TContext : IApplicationContext
    {
        IUnitOfWork<TContext> Get();
    }

    public interface IUnitOfWorkScope : IDisposable
    {
        object UserId { get; set; }

        IUnitOfWork Get();
    }
}