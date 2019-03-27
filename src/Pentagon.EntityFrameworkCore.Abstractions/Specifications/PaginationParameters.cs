// -----------------------------------------------------------------------
//  <copyright file="PaginationParameters.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    public class PaginationParameters
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public bool AreValid => PageNumber > 0 && PageSize > 0;
    }
}