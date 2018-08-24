// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWorkFactory.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    /// <summary> Represents a scoped factory for an <see cref="IUnitOfWork{TContext}" />. </summary>
    /// <typeparam name="TContext"> The type of the context. </typeparam>
    public interface IUnitOfWorkFactory<out TContext> : IUnitOfWorkFactory
            where TContext : IApplicationContext
    {
        /// <summary> Creates a new scoped <see cref="IUnitOfWork{TContext}" />. </summary>
        /// <returns> A new unit of work. </returns>
        IUnitOfWork<TContext> Create();
    }

    public interface IUnitOfWorkFactory
    {
        /// <summary> Creates a new scoped <see cref="IUnitOfWork{TContext}" />. </summary>
        /// <returns> A new unit of work. </returns>
        IUnitOfWork Create();
    }
}