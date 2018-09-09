// -----------------------------------------------------------------------
//  <copyright file="TimeStampSource.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using Abstractions;

    public class TimeStampSource : ITimeStampSource
    {
        DateTimeOffset _value;
        bool _isSet;

        /// <inheritdoc />
        public void Set(DateTimeOffset time)
        {
            _value = time;
        }

        /// <inheritdoc />
        public DateTimeOffset GetAndReset()
        {
            if (!_isSet)
                _value = DateTimeOffset.Now;

            _isSet = false;

            return _value;
        }
    }
}