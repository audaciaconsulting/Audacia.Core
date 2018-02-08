using System.Collections.Generic;

namespace Audacia.Core
{
    public interface IPage<out T>
    {
        IEnumerable<T> Data { get; }
        int TotalPages { get; }
        int TotalRecords { get; }
    }
}