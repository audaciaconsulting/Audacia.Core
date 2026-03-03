using System.Collections.Generic;

namespace Audacia.Core;

/// <summary>
/// Interface for handling the result of paging. (See <see cref="PagingRequest"/>).
/// </summary>
/// <typeparam name="T">The type of the paged data that is returned.</typeparam>
public interface IPage<out T>
{
    /// <summary>
    /// Gets a paged set of data for one page. (See <see cref="PagingRequest.PageSize"/>).
    /// </summary>
    IEnumerable<T> Data { get; }

    /// <summary>
    /// Gets the number of pages <see cref="Data"/> will span over. (See <see cref="PagingRequest.PageSize"/>).
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// Gets the total number of records, including <see cref="Data"/>. (See <see cref="PagingRequest"/>).
    /// </summary>
    int TotalRecords { get; }
}