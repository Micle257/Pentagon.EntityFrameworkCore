// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflict.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using Abstractions.Entities;

    public class ConcurrencyConflictEntity
    {
        public DateTimeOffset? UpdatedAt => (Entity as IUpdateTimeStampSupport)?.UpdatedAt;

        public string UpdatedUser => (Entity as IUpdatedUserEntitySupport)?.UpdatedUser;

        public object UpdatedUserId => (Entity as IUpdateTimeStampIdentitySupport)?.UpdatedUserId;

        public IEntity Entity { get; set; }
    }
}