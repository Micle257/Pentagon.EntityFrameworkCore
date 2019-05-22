﻿// -----------------------------------------------------------------------
//  <copyright file="TextFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications.Filters
{
    public enum TextFilter
    {
        Unspecified,
        Custom,
        Equal,
        NotEqual,
        Empty,
        NotEmpty,
        StartWith,
        EndWith,
        Contain,
        NotContain
    }
}