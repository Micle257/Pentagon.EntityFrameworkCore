// -----------------------------------------------------------------------
//  <copyright file="ITimeStampSource.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System;

    public interface ITimeStampSource
    {
        void Set(DateTimeOffset time);
        DateTimeOffset GetAndReset();
    }
}