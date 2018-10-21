// -----------------------------------------------------------------------
//  <copyright file="IConcurrencyConflictResolver.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Threading.Tasks;

    public interface IConcurrencyConflictResolver<TContext> : IConcurrencyConflictResolver
    {
    }

    public interface IConcurrencyConflictResolver
    {
        Task<ConcurrencyConflictResolveResult> ResolveAsync(IApplicationContext appContext);
    }
}