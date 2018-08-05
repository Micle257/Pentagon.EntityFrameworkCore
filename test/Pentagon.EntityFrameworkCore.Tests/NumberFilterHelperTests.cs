namespace Pentagon.EntityFrameworkCore.Tests {
    using Specifications;
    using Xunit;

    public class NumberFilterHelperTests
    {
        [Theory]
        [InlineData(NumberFilter.Equal, 12.5, 12.5, true)]
        [InlineData(NumberFilter.Equal, 12.5, 1.5, false)]
        [InlineData(NumberFilter.NotEqual, 12.5, 12.5, false)]
        [InlineData(NumberFilter.NotEqual, 12.5, 1.5, true)]
        [InlineData(NumberFilter.GreatenThan, 150, 10, true)]
        [InlineData(NumberFilter.GreatenThan, 5, 10, false)]
        [InlineData(NumberFilter.GreatenThenOrEqualTo, 150, 10, true)]
        [InlineData(NumberFilter.GreatenThenOrEqualTo, 5, 10, false)]
        [InlineData(NumberFilter.GreatenThenOrEqualTo, 5, 5, true)]
        [InlineData(NumberFilter.LessThen, 150, 10, false)]
        [InlineData(NumberFilter.LessThen, 5, 10, true)]
        [InlineData(NumberFilter.LessThenOrEqualTo, 150, 10, false)]
        [InlineData(NumberFilter.LessThenOrEqualTo, 5, 10, true)]
        [InlineData(NumberFilter.LessThenOrEqualTo, 5, 5, true)]
        public void GetFilter_ReturnsCorrectExpression(NumberFilter filterType, decimal value, decimal filterValue, bool result)
        {
            var filter = NumberFilterExpressionHelper<decimal>.GetFilter<decimal>(p => value, filterType, filterValue);

            var callback = filter.Compile();
            
            Assert.Equal(result,callback(value));
        }
    }
}