namespace Audacia.Core
{
    public interface ISortablePagingRequest : IPagingRequest
    {
        string SortProperty { get; set; }
        bool Descending { get; set; }
    }
}