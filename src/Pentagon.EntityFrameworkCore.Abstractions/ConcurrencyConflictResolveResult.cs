﻿namespace Pentagon.EntityFrameworkCore {
    using System.Collections.Generic;
    using Abstractions.Entities;

    public class ConcurrencyConflictResolveResult
    {
        public bool HasConflicts => ConflictedEntities?.Count != 0;

        public List<IEntity> ConflictedEntities { get; set; }
    }
}