namespace Audacia.Core
{
    /// <summary>
    /// Request to sort, and apply paging to a list of results.
    /// </summary>
    /// <param name="PageSize">The size of the page.</param>
    /// <param name="PageNumber">The page number.</param>
    /// <param name="SortProperty">The property to sort by.</param>
    /// <param name="Descending">Indicates if the sorting should be in descending order.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?", Justification = "Makes sense when passed from UI.")]
    public record SortablePagingRequest(int PageSize = int.MaxValue, int PageNumber = 0, string SortProperty = "", bool Descending = false) : PagingRequest(PageSize, PageNumber);
}