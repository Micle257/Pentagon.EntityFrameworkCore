namespace Pentagon.Data.EntityFramework.Synchonization {
    using Abstractions.Entities;

    public interface ISynchronizationFactory {
        ITwoWaySynchronization<T> CreateTwoWay<T>()
                where T : class, IEntity, ICreateStampSupport, ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, new();
    }
}