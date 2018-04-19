namespace Pentagon.EntityFrameworkCore.Abstractions.Entities {
    /// <summary>
    /// Represent an entity, that supports two way database synchronization.
    /// </summary>
    public interface ITwoWaySynchronizationSupport : ITimeStampSupport, IDeletedFlagSupport, IDeleteTimeStampSupport, ICreateStampSupport { }
}