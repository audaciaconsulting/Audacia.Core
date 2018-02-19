namespace Audacia.Core
{
    public class PagingRequest
    {
        public PagingRequest()
            : this(int.MaxValue, 0)
        {
            /* Required for injection */
        }

        public PagingRequest(int pageSize = int.MaxValue, int pageNumber = 0)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int? PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}