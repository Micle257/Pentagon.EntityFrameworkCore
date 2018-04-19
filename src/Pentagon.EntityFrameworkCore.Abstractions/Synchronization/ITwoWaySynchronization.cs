namespace Pentagon.EntityFrameworkCore.Synchronization {
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions.Entities;

    public interface ITwoWaySynchronization<T>
            where T : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport {
        /// <summary> Synchronizes the remote/local replicas. </summary>
        /// <param name="selector"> The selector for getting the data from db. </param>
        /// <returns> An awaitable void. </returns>
        Task SynchronizeAsync(Expression<Func<T, bool>> selector);
    }
}