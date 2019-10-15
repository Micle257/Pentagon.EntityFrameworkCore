namespace Pentagon.EntityFrameworkCore.Options {
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public class RepositoryCacheOptions
    {
        [NotNull]
        public IDictionary<string, EntityCacheOptions> Entities { get; set; } = new Dictionary<string, EntityCacheOptions>();

        [NotNull]
        public EntityCacheOptions Default { get; set; } = new EntityCacheOptions();
    }
}