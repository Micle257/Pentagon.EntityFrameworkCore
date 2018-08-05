// -----------------------------------------------------------------------
//  <copyright file="TextFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    public enum TextFilter
    {
        Unspecified,
        Equal,
        NotEqual,
        StartWith,
        EndWith,
        Contain,
        NotContain
    }
}