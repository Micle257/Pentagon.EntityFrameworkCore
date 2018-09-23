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
            var filter = NumberFilterExpressionHelper.GetFilter<decimal>(p => value, filterType, filterValue);

            var callback = filter.Compile();
            
            Assert.Equal(result,callback(value));
        }
        
        [Theory]
        [InlineData(NumberFilter.Equal, 12.5, 12.5, true)]
        [InlineData(NumberFilter.Equal, 12.5, 1.5, false)]
        [InlineData(NumberFilter.NotEqual, 12.5, 12.5, false)]
        [InlineData(NumberFilter.NotEqual, 12.5, 1.5, true)]
        [InlineData(NumberFilter.GreatenThan, 150.0, 10.0, true)]
        [InlineData(NumberFilter.GreatenThan, 5.0, 10.0, false)]
        [InlineData(NumberFilter.GreatenThenOrEqualTo, 150.0, 10.0, true)]
        [InlineData(NumberFilter.GreatenThenOrEqualTo, 5.0, 10.0, false)]
        [InlineData(NumberFilter.GreatenThenOrEqualTo, 5.0, 5.0, true)]
        [InlineData(NumberFilter.LessThen, 150.0, 10.0, false)]
        [InlineData(NumberFilter.LessThen, 5.0, 10.0, true)]
        [InlineData(NumberFilter.LessThenOrEqualTo, 150.0, 10.0, false)]
        [InlineData(NumberFilter.LessThenOrEqualTo, 5.0, 10.0, true)]
        [InlineData(NumberFilter.LessThenOrEqualTo, 5.0, 5.0, true)]
        [InlineData(NumberFilter.Empty, null, null, true)]
        [InlineData(NumberFilter.Empty, 457.0, null, false)]
        [InlineData(NumberFilter.Empty, 0, null, false)]
        [InlineData(NumberFilter.NotEmpty, null, null, false)]
        [InlineData(NumberFilter.NotEmpty, 457.0, null, true)]
        [InlineData(NumberFilter.NotEmpty, 0, 98745, true)]
        public void GetFilter_ForNullables_ReturnsCorrectExpression(NumberFilter filterType, double? value, double? filterValue, bool result)
        {
            var filter = NumberFilterExpressionHelper.GetFilter<double?>(p => value, filterType, filterValue);

            var callback = filter.Compile();
            
            Assert.Equal(result,callback(value));
        }
    }
}