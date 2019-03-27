// -----------------------------------------------------------------------
//  <copyright file="GetManySpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Abstractions.Specifications;
    using JetBrains.Annotations;

    public class InvalidPaginationParametersException : Exception
    {
        public PaginationParameters Parameters { get; }

        public InvalidPaginationParametersException(PaginationParameters parameters, string message = null) : base(message ?? $"Pagination parameters are invalid. Data: [PageNumber: {parameters.PageNumber}, PageSize: {parameters.PageSize}]")
        {
            Parameters = parameters;
        }
    }
}