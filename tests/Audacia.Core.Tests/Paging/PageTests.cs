using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Audacia.Core.Tests.Paging;

public class PageTests
{
    private record ExampleDto
    {
        public int Id { get; set; } = 0;

        public string SortColumn { get; set; }
    }

    [Fact]
    public void TotalCountPopulatedBasedOnResultCount()
    {
        var dataCount = new Random().Next(100);
        const int pageSize = 10;
        var query = Enumerable.Range(0, dataCount)
            .Select(_ => new ExampleDto())
            .AsQueryable();
        var pagingRequest = new PagingRequest(pageSize);

        var page = new Page<ExampleDto>(query, pagingRequest);

        page.TotalRecords.ShouldBe(dataCount);
    }

    [Fact]
    public void NumberOfPagesIsRoundedUpWhenPartialPageIsFilled()
    {
        const int dataCount = 11;
        const int pageSize = dataCount - 1;
        var query = Enumerable.Range(0, dataCount)
            .Select(_ => new ExampleDto())
            .AsQueryable();
        var pagingRequest = new PagingRequest(pageSize);

        var page = new Page<ExampleDto>(query, pagingRequest);

        page.TotalPages.ShouldBe(2);
    }

    [Fact]
    public void ResultsSkippedIfPageNumberIsGreaterThanOne()
    {
        const int pageSize = 1;
        const int pageNumber = 2;
        var expectedExcludedRow = new ExampleDto();
        var query = new List<ExampleDto>
        {
            expectedExcludedRow,
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 }
        };
        var pagingRequest = new PagingRequest(pageSize, pageNumber);

        var page = new Page<ExampleDto>(query, pagingRequest);

        page.Data.ShouldNotContain(expectedExcludedRow);
    }

    [Fact]
    public void AllResultsReturnedIfNoPageSizeSpecified()
    {
        var dataCount = new Random().Next(100);
        var pagingRequest = new PagingRequest();
        var query = Enumerable.Range(0, dataCount)
            .Select(_ => new ExampleDto())
            .AsQueryable();

        var page = new Page<ExampleDto>(query, pagingRequest);

        page.Data.Count().ShouldBe(dataCount);
    }

    [Fact]
    public void SortsResultsBeforeApplyingPaging()
    {
        const int pageSize = 1;
        const int pageNumber = 2;
        var pagingRequest = new SortablePagingRequest(pageSize, pageNumber)
        {
            SortProperty = nameof(ExampleDto.SortColumn),
            Descending = true
        };
        var expectedExcludedRow = new ExampleDto { SortColumn = "A" };
        var query = new List<ExampleDto>
        {
            new() { SortColumn = "C" },
            new() { SortColumn = "D" },
            expectedExcludedRow,
            new() { SortColumn = "B" }
        };

        var page = new Page<ExampleDto>(query, pagingRequest);

        page.Data.ShouldNotContain(expectedExcludedRow);
    }

    [Fact]
    public void ThrowsExceptionWhenInvalidSortPropertyProvided()
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

        act.ShouldThrow<ArgumentException>();
    }
}