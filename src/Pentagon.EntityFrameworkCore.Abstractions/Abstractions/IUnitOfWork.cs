// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWorkCommitExecutor.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Threading.Tasks;

    public interface IUnitOfWork<in TContext> : IUnitOfWork
            where TContext : IApplicationContext
    {
    }

    public interface IUnitOfWork
    {
        Task<UnitOfWorkCommitResult> ExecuteCommitAsync(IApplicationContext appContext);

        UnitOfWorkCommitResult ExecuteCommit(IApplicationContext appContext);
    }
}