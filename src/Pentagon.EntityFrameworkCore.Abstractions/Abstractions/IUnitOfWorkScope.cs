// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWorkScope.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;

    public interface IUnitOfWorkScope<out TContext> : IDisposable
            where TContext : IApplicationContext
    {
        IUnitOfWork<TContext> Get();
    }
}