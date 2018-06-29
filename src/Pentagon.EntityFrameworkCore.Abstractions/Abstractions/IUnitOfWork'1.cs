// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWork'1.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using JetBrains.Annotations;

    /// <summary> Represents an unit of work for data context and it is used to save data from related repositories. </summary>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    public interface IUnitOfWork<out TContext> : IUnitOfWork
            where TContext : IApplicationContext
    {
        /// <summary> Gets the context. </summary>
        /// <value> The <see cref="TContext" />. </value>
        [NotNull]
        TContext Context { get; }
    }
}