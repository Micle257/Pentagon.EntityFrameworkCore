// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflict.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using Interfaces.Entities;
    using JetBrains.Annotations;

    public class ConcurrencyConflictEntity
    {
        public DateTimeOffset? UpdatedAt => (Entity as IUpdateTimeStampSupport)?.UpdatedAt;

        public string UpdatedUser => (Entity as IUpdatedUserEntitySupport)?.UpdatedUser;

        [CanBeNull]
        public object UpdatedUserId => (Entity as IUpdateTimeStampIdentitySupport)?.UpdatedUserId;

        public object Entity { get; set; }
    }
}