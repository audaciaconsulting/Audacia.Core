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
            var specification = new PagingSpecification<T>(query)
                .WithPaging(sortablePagingRequest);

            // Get total count before paging is applied
            TotalRecords = specification.Query.Count();
            specification = specification
                .ApplySorting(sortablePagingRequest)
                .ApplyPaging();
            TotalPages = specification.GetTotalPages(TotalRecords);
            Data = specification.Query.ToList();
        }

        public Page(IQueryable<T> query, PagingRequest pagingRequest)
        {
            var specification = new PagingSpecification<T>(query)
                .WithPaging(pagingRequest);

            // Get total count before paging is applied
            TotalRecords = specification.Query.Count();
            specification = specification.ApplyPaging();
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