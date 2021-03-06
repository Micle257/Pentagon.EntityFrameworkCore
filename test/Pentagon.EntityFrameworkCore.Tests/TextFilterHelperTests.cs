namespace Pentagon.EntityFrameworkCore.Tests {
    using Mocks;
    using Specifications;
    using Xunit;

    public class TextFilterHelperTests
    {
       // [Theory]
       // [InlineData(TextFilter.Equal,"Doe", "Doe", true)]
       // [InlineData(TextFilter.Equal,"Does", "Doe", false)]
       // [InlineData(TextFilter.NotEqual, "Doe", "Doe", false)]
       // [InlineData(TextFilter.NotEqual, "Does", "Doe", true)]
       // [InlineData(TextFilter.Contain, "Vbasse", "bass", true)]
       // [InlineData(TextFilter.Contain, "basse", "bass", true)]
       // [InlineData(TextFilter.Contain, "Vbass", "bass", true)]
       // [InlineData(TextFilter.Contain, "Vbass", "basse", false)]
       // [InlineData(TextFilter.NotContain, "Vbasse", "bass", false)]
       // [InlineData(TextFilter.NotContain, "Vbass", "qwe", true)]
       // [InlineData(TextFilter.StartWith, "Basse", "Bas", true)]
       // [InlineData(TextFilter.StartWith, "wBas", "Bas", false)]
       // [InlineData(TextFilter.EndWith, "Basse", "sse", true)]
       // [InlineData(TextFilter.EndWith, "wBas", "wBa", false)]
       // [InlineData(TextFilter.EndWith, "wBas", "wBa", false)]
       // [InlineData(TextFilter.Empty, "", null, true)]
       // [InlineData(TextFilter.Empty, "   ", null, true)]
       // [InlineData(TextFilter.Empty, null, null, true)]
       // [InlineData(TextFilter.Empty, "lol", null, false)]
       // [InlineData(TextFilter.NotEmpty, "", null, false)]
       // [InlineData(TextFilter.NotEmpty, "   ", null, false)]
       // [InlineData(TextFilter.NotEmpty, null, null, false)]
       // [InlineData(TextFilter.NotEmpty, "lol", null, true)]
       // public void GetFilter_ReturnsCorrectExpression(TextFilter filterType,string value, string filterValue, bool result)
       // {
       //     var composite = new CompositeFilter<Person, TextFilter, string>
       //                     {
       //                             Property = p => p.Name,
       //                             FirstCondition = filterType,
       //                             FirstValue = filterValue
       //                     };
       //     
       //     var person = new Person { Name = value};
       //
       //     var filter = FilterExpressionHelper.GetFilter(composite);
       //
       //     var callback = filter.Compile();
       //     
       //     Assert.Equal(result,callback(person));
       // }
    }
}