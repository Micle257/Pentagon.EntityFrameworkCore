// -----------------------------------------------------------------------
//  <copyright file="IUnitOfWorkCommitExecutor.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Threading.Tasks;

    public interface IUnitOfWorkCommitExecutor<in TContext>
            where TContext : IApplicationContext
    {
        Task<UnitOfWorkCommitResult> ExecuteCommitAsync(IUnitOfWork<TContext> unitOfWork);

        UnitOfWorkCommitResult ExecuteCommit(IUnitOfWork<TContext> unitOfWork);
    }
}