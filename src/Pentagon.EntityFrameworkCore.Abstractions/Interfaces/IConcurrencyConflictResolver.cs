// -----------------------------------------------------------------------
//  <copyright file="IConcurrencyConflictResolver.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    
    public interface IConcurrencyConflictResolver
    {
        Task<ConcurrencyConflictResolveResult> ResolveAsync([NotNull] IApplicationContext appContext,
                                                            [NotNull] Func<IApplicationContext> contextFactory);
    }
}