// -----------------------------------------------------------------------
//  <copyright file="NumberFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications.Filters
{
    public enum NumberFilter 
    {
        Unspecified,
        Equal,
        NotEqual, 
        Empty,
        NotEmpty,
        GreatenThan,
        GreatenThenOrEqualTo,
        LessThen,
        LessThenOrEqualTo
    }
}