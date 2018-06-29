// -----------------------------------------------------------------------
//  <copyright file="IContextFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    /// <summary> Represents a context factory for <see cref="IApplicationContext" />s. </summary>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    public interface IContextFactory<out TContext>
            where TContext : IApplicationContext
    {
        /// <summary> Creates the context. </summary>
        /// <param name="args"> The arguments. </param>
        /// <returns> A <see cref="TContext" />. </returns>
        TContext CreateContext(string[] args = null);
    }
}