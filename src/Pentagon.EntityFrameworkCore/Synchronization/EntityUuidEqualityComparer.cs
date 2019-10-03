namespace Pentagon.EntityFrameworkCore.Synchronization {
    using System.Collections.Generic;
    using Interfaces.Entities;

    class EntityUuidEqualityComparer<T> : IEqualityComparer<T>
            where T : class, ICreateStampSupport
    {
        public static EntityUuidEqualityComparer<T> Instance { get; } = new EntityUuidEqualityComparer<T>();

        /// <inheritdoc />
        public bool Equals(T x, T y) => x.Uuid == y.Uuid;

        /// <inheritdoc />
        public int GetHashCode(T obj) => obj.Uuid.GetHashCode();
    }
}