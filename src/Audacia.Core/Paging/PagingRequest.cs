namespace Audacia.Core
{
    /// <summary>
    /// Request to apply paging to a list of results.
    /// </summary>
    /// <param name="PageSize"> Gets or sets the number of results we'll show per page. </param>
    /// <param name="PageNumber"> Gets or sets which page of results to return. Pages start at 1. </param>
    public record PagingRequest(int PageSize = int.MaxValue, int PageNumber = 0);
}