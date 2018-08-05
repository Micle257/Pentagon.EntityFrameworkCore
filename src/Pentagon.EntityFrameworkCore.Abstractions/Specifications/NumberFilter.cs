// -----------------------------------------------------------------------
//  <copyright file="NumberFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    public enum NumberFilter
    {
        Unspecified,
        Equal,
        NotEqual,
        GreatenThan,
        GreatenThenOrEqualTo,
        LessThen,
        LessThenOrEqualTo
    }
}