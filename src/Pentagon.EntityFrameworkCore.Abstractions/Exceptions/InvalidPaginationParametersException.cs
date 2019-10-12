// -----------------------------------------------------------------------
//  <copyright file="GetManySpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Exceptions
{
    using System;
    using Specifications;

    public class InvalidPaginationParametersException : Exception
    {
        public PaginationParameters Parameters { get; }

        public InvalidPaginationParametersException(PaginationParameters parameters, string message = null) : base(message ?? $"Pagination parameters are invalid. Data: [PageNumber: {parameters.PageNumber}, PageSize: {parameters.PageSize}]")
        {
            Parameters = parameters;
        }
    }
}