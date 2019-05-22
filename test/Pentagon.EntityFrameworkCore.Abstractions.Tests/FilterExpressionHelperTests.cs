// -----------------------------------------------------------------------
//  <copyright file="FilterExpressionHelperTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.EntityFrameworkCore.Abstractions.Tests {
    using System;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications.Filters;
    using Xunit;

    public class FilterExpressionHelperTests
    {
        [Fact]
        public void GetObjectPropertySelector()
        {
            Expression<Func<IEntity, string>> sel = entity => entity.ToString();

            var objectPropertySelector = FilterExpressionHelper.GetObjectPropertySelector(sel);
        }

        [Fact]
        public void GetStringPropertySelector()
        {
            Expression<Func<string, int>> sel = str => (str == null ? 1 : 2);

            var stringPropertySelector = FilterExpressionHelper.GetStringPropertySelector(sel);

            var case1 = stringPropertySelector.Compile()(null);
            var case2 = stringPropertySelector.Compile()("");

            Assert.Equal("1", case1);
            Assert.Equal("2", case2);
        }
    }
}