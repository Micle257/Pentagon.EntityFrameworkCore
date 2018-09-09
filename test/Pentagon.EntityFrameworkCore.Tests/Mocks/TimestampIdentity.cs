namespace Pentagon.EntityFrameworkCore.Tests.Mocks {
    using System;
    using EntityFrameworkCore;

    public class TimestampIdentity : TimestampIdentityEntity<Guid?>
    {
        public string Value { get; set; }
    }
}