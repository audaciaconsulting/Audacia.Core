using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Audacia.Core.Tests.Paging
{
    public class PageTests
    {
        private class ExampleDto
        {
            public string SortColumn { get; set; }    
        }
        
        [Fact]
        public void Total_count_populated_based_on_result_count()
        {
            var dataCount = new Random().Next(100);
            const int pageSize = 10;
            var query = Enumerable.Range(0, dataCount)
                .Select(_ => new ExampleDto())
                .AsQueryable();
            var pagingRequest = new PagingRequest(pageSize);

            var page = new Page<ExampleDto>(query, pagingRequest);

            page.TotalRecords.Should().Be(dataCount);
        }
        
        [Fact]
        public void Number_of_pages_is_rounded_up_when_partial_page_is_filled()
        {
            const int dataCount = 11;
            const int pageSize = dataCount - 1;
            var query = Enumerable.Range(0, dataCount)
                .Select(_ => new ExampleDto())
                .AsQueryable();
            var pagingRequest = new PagingRequest(pageSize);

            var page = new Page<ExampleDto>(query, pagingRequest);

            page.TotalPages.Should().Be(2);
        }
        
        [Fact]
        public void Results_skipped_if_page_number_is_greater_than_one()
        {
            const int pageSize = 1;
            const int pageNumber = 2;
            var expectedExcludedRow = new ExampleDto();
            var query = new List<ExampleDto>
            {
                expectedExcludedRow,
                new ExampleDto(),
                new ExampleDto(),
                new ExampleDto()
            };
            var pagingRequest = new PagingRequest(pageSize, pageNumber);

            var page = new Page<ExampleDto>(query, pagingRequest);

            page.Data.Should().NotContain(expectedExcludedRow);
        }
        
        [Fact]
        public void All_results_returned_if_no_page_size_specified()
        {
            var dataCount = new Random().Next(100);
            var pagingRequest = new PagingRequest();
            var query = Enumerable.Range(0, dataCount)
                .Select(_ => new ExampleDto())
                .AsQueryable();

            var page = new Page<ExampleDto>(query, pagingRequest);

            page.Data.Should().HaveCount(dataCount);
        }

        [Fact]
        public void Sorts_results_before_applying_paging()
        {
            const int pageSize = 1;
            const int pageNumber = 2;
            var pagingRequest = new SortablePagingRequest(pageSize, pageNumber)
            {
                SortProperty = nameof(ExampleDto.SortColumn),
                Descending = true
            };
            var expectedExcludedRow = new ExampleDto{ SortColumn = "A"};
            var query = new List<ExampleDto>
            {
                new ExampleDto{ SortColumn = "C"},
                new ExampleDto{ SortColumn = "D"},
                expectedExcludedRow,
                new ExampleDto { SortColumn = "B"},
            };

            var page = new Page<ExampleDto>(query, pagingRequest);

            page.Data.Should().NotContain(expectedExcludedRow);
        }

        [Fact]
        public void Throws_exception_when_invalid_sort_property_provided()
        {
            const int pageSize = 1;
            var pagingRequest = new SortablePagingRequest(pageSize)
            {
                SortProperty = Guid.NewGuid().ToString()
            };
            var dataCount = new Random().Next(100);
            var query = Enumerable.Range(0, dataCount)
                .Select(_ => new ExampleDto())
                .AsQueryable();

            Func<Page<ExampleDto>> act = () => new Page<ExampleDto>(query, pagingRequest);

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}