// -----------------------------------------------------------------------
//  <copyright file="IApplicationContext.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using Repositories;

    /// <summary> Represents a database application context. </summary>
    public interface IApplicationContext : IDisposable
    {
        bool UseTimeSourceFromEntities { get; set; }

        bool AutoResolveConflictsFromSameUser { get; set; }

        event EventHandler<CommitEventArgs> Commiting;

        Task<ContextCommitResult> ExecuteCommitAsync(CancellationToken cancellationToken = default);

        ContextCommitResult ExecuteCommit();

        /// <summary> Gets the repository of given entity. </summary>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <returns> A <see cref="IRepository{TEntity}" />. </returns>
        IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new();
    }
}