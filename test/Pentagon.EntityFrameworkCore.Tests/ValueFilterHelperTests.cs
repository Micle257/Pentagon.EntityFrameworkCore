// -----------------------------------------------------------------------
//  <copyright file="ValueFilterHelperTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Tests
{
    using Abstractions.Entities;
    using Mocks;
    using Specifications;
    using Specifications.Filters;
    using Xunit;

    public class ValueFilterHelperTests
    {
        [Theory]
        [InlineData("Doe", new[] {"Doe"}, true)]
        [InlineData("Doe", new[] {"Does"}, false)]
        [InlineData("Doe", new[] {"Doe", "Lo", "age"}, true)]
        [InlineData("Doe", new[] {"Lo", "Doe"}, true)]
        [InlineData("Doe", new[] {"Lo", "age"}, false)]
        public void GetFilter_ReturnsCorrectExpression(string value, string[] filterValues, bool result)
        {
            var person = new Person {Name = value};

            var filter = ValueFilterExpressionHelper.GetFilter<Person, string>(p => p.Name, filterValues);

            var callback = filter.Compile();

            Assert.Equal(result, callback(person));
        }
    }
}