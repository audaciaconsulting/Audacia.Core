namespace Audacia.Core
{
/// <summary>
    /// Request to sort, and apply paging to a list of results.
    /// </summary>
    public record SortablePagingRequest : PagingRequest
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SortablePagingRequest"/>.
        /// </summary>
        public SortablePagingRequest()
            : this(int.MaxValue, 0)
        {
            /* Required for injection */
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SortablePagingRequest"/>.
        /// </summary>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        public SortablePagingRequest(int pageSize = int.MaxValue, int pageNumber = 0)
            : base(pageSize, pageNumber)
        {
            SortProperty = string.Empty;
        }

        /// <summary>
        /// Gets or sets the property of the destination type we'll sort by.
        /// </summary>
        public string SortProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value indicating the sort direction.
        /// </summary>
        public bool Descending { get; set; }
    }
}