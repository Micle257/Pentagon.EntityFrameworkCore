namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using System;
    using Abstractions.Entities;
    using EntityFrameworkCore;

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