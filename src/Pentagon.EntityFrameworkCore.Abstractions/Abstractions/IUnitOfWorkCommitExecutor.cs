// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWorkCommitExecutor.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Threading.Tasks;

    public interface IUnitOfWorkCommitExecutor<in TContext> : IUnitOfWorkCommitExecutor
            where TContext : IApplicationContext
    {
    }

    public interface IUnitOfWorkCommitExecutor
    {
        Task<UnitOfWorkCommitResult> ExecuteCommitAsync(IApplicationContext appContext);

        UnitOfWorkCommitResult ExecuteCommit(IApplicationContext appContext);
    }
}