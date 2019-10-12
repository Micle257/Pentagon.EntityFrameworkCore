// -----------------------------------------------------------------------
//  <copyright file="PageOutOfRangeException.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Exceptions
{
    using System;

    public class PageOutOfRangeException : ArgumentOutOfRangeException
    {
        public PageOutOfRangeException(string paramName, int pageNumber, int pageCount) : base(paramName: paramName, message: "The page number is out of range.", actualValue: pageNumber)
        {
            PageCount = pageCount;
        }

        public int PageCount { get; }
    }
}