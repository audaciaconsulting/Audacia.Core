namespace Audacia.Core
{
    /// <summary>
    /// Request to sort, and apply paging to a list of results.
    /// </summary>
    public class SortablePagingRequest : PagingRequest
    {
        public SortablePagingRequest()
            : this(int.MaxValue, 0)
        {
            /* Required for injection */
        }

        public SortablePagingRequest(int pageSize = int.MaxValue, int pageNumber = 0)
            : base(pageSize, pageNumber)
        {
        }

        /// <summary>
        /// Gets or sets the property of the destination type we'll sort by.
        /// </summary>
        public string SortProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the sort direction.
        /// </summary>
        public bool Descending { get; set; }
    }
}