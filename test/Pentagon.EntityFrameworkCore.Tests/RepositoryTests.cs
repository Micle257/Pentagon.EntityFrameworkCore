// -----------------------------------------------------------------------
//  <copyright file="RepositoryTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Tests
{
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using Mocks;
    using Repositories;
    using Specifications;
    using Xunit;

    public class RepositoryTests
    {
        [Fact]
        public void GetPageAsync_Scenario_ExpectedBehavior()
        {
            var c = new Context();

            var data = new[]
                       {
                               new Person
                               {
                                       Name = "Pete",
                                       Age = 12
                               },
                               new Person
                               {
                                       Name = "Pete",
                                       Age = 46
                               },
                               new Person
                               {
                                       Name = "Zeta",
                                       Age = 21
                               },new Person
                                 {
                                         Name = "Ale",
                                         Age = 43
                                 },
                       };

            var re = new Repository<Person>(c.Set<Person>());

            foreach (var simple in data)
                re.Insert(simple);

            c.SaveChanges();

            var spec = new GetManySpecification<Person>();

            spec.AddOrder(p => p.Name, false)
                .AddOrder(p => p.Age, true);

            var ma = re.GetManyAsync(spec).Result.ToList();

            Assert.Equal("Ale", ma[0].Name);
            Assert.Equal("Pete", ma[1].Name);
            Assert.Equal(46, ma[1].Age);
            Assert.Equal(12, ma[2].Age);
            Assert.Equal("Zeta", ma[3].Name);

            var filterSpecification = new GetManySpecification<Person>();

            filterSpecification.AddFilter(b => b.AddCompositeFilter(p => p.Name, TextFilter.Contain, "et")
                                                .AddCompositeFilter(p => p.Name, TextFilter.Contain, "et")
                                                .AddSubFilter(FilterLogicOperation.Or, TextFilter.StartWith, "A"));
        }
    }
}