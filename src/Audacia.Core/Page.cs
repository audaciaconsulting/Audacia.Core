using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Audacia.Core.Extensions;

namespace Audacia.Core
{
    public class Page<T> : IPage<T>
    {
        public int TotalPages { get; private set; }
        public int TotalRecords { get; private set; }
        
        public Page(IQueryable<T> query, SortablePagingRequest sortablePagingRequest)
        {
            var (pageNumber, pageSize) = PageBase(query, sortablePagingRequest);

            var descending = sortablePagingRequest.Descending;

            //Support the propertyname being set and the expression not, by converting the property name to lambda
            var sortExpressions = sortablePagingRequest is SortablePagingRequest<T> sortableExpressionPagingRequest
                ? sortableExpressionPagingRequest.SortExpressions
                : new List<Expression<Func<T, object>>>
                {
                    sortablePagingRequest.SortProperty.ToPredicate<T>()
                };
            query = SortQuery(query, sortExpressions, descending);


            Data = query.Skip(pageNumber * pageSize).Take(pageSize).ToList();
        }

        public Page(IEnumerable<T> enumerable, SortablePagingRequest
            sortablePagingRequest)
            : this(enumerable.AsQueryable(), sortablePagingRequest)
        {
        }

        public Page(IQueryable<T> query, PagingRequest pagingRequest)
        {
            var (pageNumber, pageSize) = PageBase(query, pagingRequest);

            Data =
                query.Skip(pageNumber * pageSize).Take(pageSize).ToList();
        }

        public Page(IEnumerable<T> enumerable, PagingRequest pagingRequest)
            : this(enumerable.AsQueryable(), pagingRequest)
        {
        }

        private (int, int) PageBase(IQueryable<T> query, PagingRequest pagingRequest)
        {
            TotalRecords = query.Count();

            //If no page size specificed, show all
            var pageSize = pagingRequest.PageSize ?? int.MaxValue;

            TotalPages = Math.Max((int)Math.Ceiling(TotalRecords / (double)pageSize), 1);

            var pageNumber = pagingRequest.PageNumber > TotalPages ? 1 : pagingRequest.PageNumber;

            return (pageNumber, pageSize);
        }

        private IQueryable<T> SortQuery(IQueryable<T> query, List<Expression<Func<T, object>>> sortProperties, bool descending)
        {
            if (sortProperties == null)
            {
                return query;
            }

            foreach (var sortProperty in sortProperties)
            {
                query = query.AppendOrderBy(sortProperty, descending);
            }

            return query;
        }

        public IEnumerable<T> Data { get; }
    }
}