using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Audacia.Core.Tests
{
    [TestClass]
    public class PageTests
    {
        [TestClass]
        public class OrderBySortExpressionAndDirectionTests
        {
            [TestMethod]
            public void SortAscending_UsesSuppliedOrderingExpression()
            {
                var sortablePagingRequest = new SortablePagingRequest<TestClass>
                {
                    SortExpressions = { t => t.Description }
                };
                var result = GetPage(sortablePagingRequest);
                Assert.AreEqual(result.Data.First().Id, 3, "Should have ordered by name, then id");
            }
            [TestMethod]
            public void SortDescending_UsesSuppliedOrderingExpression()
            {
                var sortablePagingRequest = new SortablePagingRequest<TestClass>
                {
                    SortExpressions =
                    {
                        t => t.Description
                    },
                    Descending = true
                };
                var result = GetPage(sortablePagingRequest);
                Assert.AreEqual(result.Data.First().Id, 2, "Should have ordered by name, then id");
            }
            [TestMethod]
            public void SortAscending_CombinesOrdering()
            {
                var sortablePagingRequest = new SortablePagingRequest<TestClass>
                {
                    SortExpressions = { t => t.Name, t => t.Id }
                };
                var result = GetPage(sortablePagingRequest);
                Assert.AreEqual(result.Data.First().Id, 2, "Should have ordered by name, then id");
            }

            [TestMethod]
            public void SortDescending_CombinesOrdering()
            {
                var sortablePagingRequest = new SortablePagingRequest<TestClass>
                {
                    SortExpressions = { t => t.Name, t => t.Id },
                    Descending = true
                };
                var result = GetPage(sortablePagingRequest);

                //Entity with the lowest ID & lowest name
                Assert.AreEqual(result.Data.First().Id, 4, "Should have ordered by name, then id");
            }

            [TestMethod]
            public void SetSortExpressionUsed_UsesSortExpression()
            {
                //Check we still sort by name when a string is provided
                var sortablePagingRequest = new SortablePagingRequest<TestClass>();
                sortablePagingRequest.SetSortExpression($"{nameof(TestClass.Name)}");

                var result = GetPage(sortablePagingRequest);

                Assert.AreEqual(result.Data.First().Id, 6, "Should have ordered by name when SetSortExpression is used");
            }

            [TestMethod]
            public void NonGenericSorterUsed_UsesSortExpression()
            {
                //use the old PagingRequest object
                var sortablePagingRequest = new SortablePagingRequest
                {
                    SortProperty = $"{nameof(TestClass.Name)}"
                };
                var result = GetPage(sortablePagingRequest);

                //Entity with the lowest ID & lowest name
                Assert.AreEqual(result.Data.First().Id, 6, "Should have converted the provided property name to lambda expression");
            }


            [TestMethod]
            public void LowerCaseSortPropertyUsed_UsesSortExpression()
            {
                //use the old PagingRequest object
                var sortablePagingRequest = new SortablePagingRequest
                {
                    SortProperty = $"{nameof(TestClass.Name).ToLower()}"
                };
                var result = GetPage(sortablePagingRequest);

                //Entity with the lowest ID & lowest name
                Assert.AreEqual(result.Data.First().Id, 6, "Should have converted the provided lower-case property name to lambda expression");
            }

            private static Page<TestClass> GetPage(SortablePagingRequest sortablePagingRequest)
            {
                var items = new List<TestClass>
                {
                    new TestClass {Id = 1, Name = "B", Description="W"},
                    new TestClass {Id = 4, Name = "B", Description="X"},
                    new TestClass {Id = 3, Name = "B", Description="U"},
                    new TestClass {Id = 6, Name = "A", Description="V"},
                    new TestClass {Id = 2, Name = "A", Description="Z"},
                    new TestClass {Id = 5, Name = "A", Description="Y"}
                }.AsQueryable();

                return new Page<TestClass>(items, sortablePagingRequest);
            }

            private class TestClass
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string Description { get; set; }
            }

        }
    }

}
