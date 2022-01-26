using System.Collections.Generic;

namespace Audacia.Core.Paging
{
    /// <summary>
    /// Extensions to <see cref="IEnumerable{T}"/> pertaining to paging queries.
    /// </summary>
    public static class PagingEnumerableExtensions
    {
        /// <summary>
        /// Gets a page of data from the given <paramref name="query"/>, where the <paramref name="pagingRequest"/> contains the page size, etc.
        /// </summary>
        /// <typeparam name="T">The type of data contained in the given <paramref name="query"/>.</typeparam>
        /// <param name="query">The query to page.</param>
        /// <param name="pagingRequest">The object containing paging-related parameters.</param>
        /// <returns>A page of data containing items of type <typeparamref name="T"/>.</returns>
        public static IPage<T> ToPage<T>(this IEnumerable<T> query, SortablePagingRequest pagingRequest)
        {
            return new Page<T>(query, pagingRequest);
        }
    }
}
