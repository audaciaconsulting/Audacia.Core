using System.Collections.Generic;
using System.Linq;

namespace Audacia.Core
{
    /// <summary>
    /// A results page of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    public class Page<T> : IPage<T>
    {
        /// <summary>
        /// Gets the number of pages in the data before being filtered.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "AV1710:Member name includes the name of its containing type",
            Justification = "The 'TotalPages' property in the 'Page' class does not merely repeat the type name, but instead represents a different concept: the total number of 'Page' instances in a higher-level object. The name is chosen to clearly express this concept and make the code more understandable.")]
        public int TotalPages { get; }

        /// <summary>
        /// Gets the number of records in the data before being filtered.
        /// </summary>
        public int TotalRecords { get; }

        /// <summary>
        /// Gets a collection of records after sorting and paging has been applied.
        /// </summary>
        public IEnumerable<T> Data { get; }

        /// <summary>
        /// A results page of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="query">A LINQ query.</param>
        /// <param name="sortablePagingRequest">Request to sort and apply paging to a query.</param>
        public Page(IQueryable<T> query, SortablePagingRequest sortablePagingRequest)
        {
            // Get total count before paging is applied
            TotalRecords = query.Count();

            var specification = new PagingSpecification<T>(query)
                .ConfigurePaging(sortablePagingRequest)
                .ConfigureSorting(sortablePagingRequest)
                .UseSorting()
                .UsePaging();

            TotalPages = specification.GetTotalPages(TotalRecords);
            Data = specification.Query.ToList();
        }

        /// <summary>
        /// A results page of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="query">A LINQ query.</param>
        /// <param name="pagingRequest">Request apply paging to a query.</param>
        public Page(IQueryable<T> query, PagingRequest pagingRequest)
        {
            // Get total count before paging is applied
            TotalRecords = query.Count();

            var specification = new PagingSpecification<T>(query)
                .ConfigurePaging(pagingRequest)
                .UsePaging();

            TotalPages = specification.GetTotalPages(TotalRecords);
            Data = specification.Query.ToList();
        }

        /// <summary>
        /// A results page of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="enumerable">A collection of data.</param>
        /// <param name="totalPages">The number of pages in the data before being filtered.</param>
        /// <param name="totalRecords">The number of records in the data before being filtered.</param>
        public Page(IEnumerable<T> enumerable, int totalPages, int totalRecords)
        {
            Data = enumerable;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
        }

        /// <summary>
        /// A results page of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="enumerable">A collection of data.</param>
        /// <param name="sortablePagingRequest">Request to sort and apply paging to a query.</param>
        public Page(IEnumerable<T> enumerable, SortablePagingRequest sortablePagingRequest)
            : this(enumerable.AsQueryable(), sortablePagingRequest)
        {
        }

        /// <summary>
        /// A results page of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="enumerable">A collection of data.</param>
        /// <param name="pagingRequest">Request apply paging to a query.</param>
        public Page(IEnumerable<T> enumerable, PagingRequest pagingRequest)
            : this(enumerable.AsQueryable(), pagingRequest)
        {
        }
    }
}