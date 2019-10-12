namespace Pentagon.EntityFrameworkCore.Tests
{
    using System.Linq;
    using EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;
    using Entity = Mocks.Entity;

    public class EntityHelperTests
    {
        [Fact]
        public void GetPureProperties_ForGivenEntity_ReturnCorrectProperties()
        {
            var en = new Entity();

            var props = EntityHelper.GetPureProperties(en)
                                    .Select(a => a.Name);

            Assert.Equal(new[] { "Value" }, props);
        }
    }
}