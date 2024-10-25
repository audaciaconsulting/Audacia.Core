namespace Audacia.Core
{
    /// <summary>
    /// Request to apply paging to a list of results.
    /// </summary>
    public record PagingRequest
    {
        /// <summary>
        /// Request to apply paging to a list of results with default values.
        /// </summary>
        public PagingRequest()
            : this(int.MaxValue, 0)
        {
            /* Required for injection */
        }

        /// <summary>
        /// Request to apply paging to a list of results.
        /// </summary>
        /// <param name="pageSize">The number of results to show per page.</param>
        /// <param name="pageNumber">Which page of results to return.</param>
        public PagingRequest(int pageSize = int.MaxValue, int pageNumber = 0)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        /// <summary>
        /// Gets or sets the number of results we'll show per page.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Gets or sets which page of results to return. Pages start at 1.
        /// </summary>
        public int PageNumber { get; set; } = 0;
    }
}