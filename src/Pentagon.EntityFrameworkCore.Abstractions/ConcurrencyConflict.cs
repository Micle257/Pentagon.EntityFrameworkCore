// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflict.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Abstractions.Entities;

    public class ConcurrencyConflict
    {
        public IUpdatedTimeStampSupport EntityUpdated => Entity as IUpdatedTimeStampSupport;

        public ITimeStampIdentitySupport EntityIdentity => Entity as ITimeStampIdentitySupport;

        public IEntity Entity { get; set; }
    }
}