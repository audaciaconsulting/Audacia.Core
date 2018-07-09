namespace Audacia.Core
{
    public interface IPagingRequest
    {
        int PageNumber { get; set; }
        int? PageSize { get; set; }
    }
}