// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflict.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Abstractions.Entities;

    public class ConcurrencyConflictEntity
    {
        public IUpdateTimeStampSupport EntityUpdated => Entity as IUpdateTimeStampSupport;

        public IUpdateTimeStampIdentitySupport EntityIdentity => Entity as IUpdateTimeStampIdentitySupport;

        public IEntity Entity { get; set; }
    }
}