namespace Pentagon.EntityFrameworkCore.Options {
    using System;

    public class EntityCacheOptions
    {
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(10);

        public TimeSpan? SlidingExpiration { get; set; }
    }
}