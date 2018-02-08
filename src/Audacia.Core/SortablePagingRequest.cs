namespace Audacia.Core
{
    public class SortablePagingRequest : PagingRequest
    {
        public SortablePagingRequest(int pageSize = int.MaxValue, int pageNumber = 0)
            : base(pageSize, pageNumber)
        {
        }

        public string SortProperty { get; set; }

        public bool Descending { get; set; }
    }
}