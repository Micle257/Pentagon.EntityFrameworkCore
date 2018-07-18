// -----------------------------------------------------------------------
//  <copyright file="RepositoryTests.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Pentagon.Data.EntityFramework.Tests
{
    using System.Linq;
    using EntityFrameworkCore;
    using EntityFrameworkCore.Repositories;
    using EntityFrameworkCore.Specifications;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class RepositoryTests
    {
        [Fact]
        public void GetPageAsync_Scenario_ExpectedBehavior()
        {
            var c = new Context();
            var data = new Simple[]
                       {
                           new Simple {Data = "hi"},
                           new Simple {Data = "hsadi"},
                           new Simple {Data = "hid"},
                           new Simple {Data = "hidc"},
                           new Simple {Data = "hiw"},
                           new Simple {Data = "hixz"},
                           new Simple {Data = "absd"},
                           new Simple {Data = "hcxz"},
                           new Simple {Data = "xxxx"},
                           new Simple {Data = "wwww"},
                       };
            var re = new Repository<Simple>(NullLogger<Repository<Simple>>.Instance, new PaginationService(), c);
            
            foreach (var simple in data)
            {
                re.Insert(simple);
            }
            c.SaveChanges();

            var list = re.GetAllPagesAsync(new GetAllPagesSpecification<Simple>(a => true, s => s.Data, false, 3)).Result.ToList();

        }
    }
}