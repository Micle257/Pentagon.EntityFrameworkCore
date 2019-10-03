namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using System;
    using EntityFrameworkCore;
    using Interfaces.Entities;

    public class Entity : TimestampIdentityEntity<int?>, IConcurrencyStampSupport, ICreatedUserEntitySupport, IUpdatedUserEntitySupport,IDeletedUserEntitySupport
    {
        public string Value { get; set; }

        /// <inheritdoc />
        public Guid ConcurrencyStamp { get; set; }

        /// <inheritdoc />
        public string CreatedUser { get; set; }

        /// <inheritdoc />
        public string UpdatedUser { get; set; }

        /// <inheritdoc />
        public string DeletedUser { get; set; }
    }
}