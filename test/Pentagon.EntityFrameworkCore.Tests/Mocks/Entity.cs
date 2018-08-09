namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using System;
    using Abstractions.Entities;
    using EntityFrameworkCore;

    public class Entity : TimestampIdentityEntity<int?>, IConcurrencyStampSupport
    {
        public string Value { get; set; }

        /// <inheritdoc />
        public Guid ConcurrencyStamp { get; set; }
    }
}