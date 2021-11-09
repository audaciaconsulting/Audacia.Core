using System.Collections.Generic;
using System.Linq;

namespace Audacia.Core
{
    /// <summary>
    /// A results page of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> : IPage<T>
    {
        public int TotalPages { get; }
        public int TotalRecords { get; }
        public IEnumerable<T> Data { get; }

        public Page(IQueryable<T> query, SortablePagingRequest sortablePagingRequest)
        {
            var specification = new PagingSpecification<T>(query);
            
            // Get total count before paging is applied
            TotalRecords = specification.Query.Count();
            
            specification = specification
                .ConfigurePaging(sortablePagingRequest)
                .ConfigureSorting(sortablePagingRequest)
                .UseSorting()
                .UsePaging();
            TotalPages = specification.GetTotalPages(TotalRecords);
            Data = specification.Query.ToList();
        }

        public Page(IQueryable<T> query, PagingRequest pagingRequest)
        {
            // Get total count before paging is applied
            var specification = new PagingSpecification<T>(query);
            TotalRecords = specification.Query.Count();

            specification = specification
                .ConfigurePaging(pagingRequest)
                .UsePaging();
            TotalPages = specification.GetTotalPages(TotalRecords);
            Data = specification.Query.ToList();
        }

        public Page(IEnumerable<T> enumerable, int totalPages, int totalRecords)
        {
            Data = enumerable;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
        }

        public Page(IEnumerable<T> enumerable, SortablePagingRequest sortablePagingRequest)
            : this(enumerable.AsQueryable(), sortablePagingRequest)
        {
        }
        
        public Page(IEnumerable<T> enumerable, PagingRequest pagingRequest)
            : this(enumerable.AsQueryable(), pagingRequest)
        {
        }
    }
}