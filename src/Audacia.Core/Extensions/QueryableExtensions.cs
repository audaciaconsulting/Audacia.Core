using System;
using Audacia.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audacia.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Core.ExtensionMethods
{
    public static class QueryableExtensions
    {
        public static IPage<T> Page<T>(this IQueryable<T> query, ISortablePagingRequest pagingRequest)
        {
            var totalRecords = query.Count();

            var (pageSize, totalPages, pageNumber) = PageBase(pagingRequest, totalRecords);

            query = Page<T>.OrderBySortPropertyAndDirection(query, pagingRequest.SortProperty, pagingRequest.Descending);

            var data = query.Skip(pageNumber * pageSize).Take(pageSize).ToList();

            return new
            {
                Data = data,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            } as IPage<T>;
        }

        public static async Task<IPage<T>> PageAsync<T>(this IQueryable<T> query, ISortablePagingRequest pagingRequest,
            CancellationToken cancellationToken)
        {
            var totalRecords = await query.CountAsync(cancellationToken);

            var (pageSize, totalPages, pageNumber) = PageBase(pagingRequest, totalRecords);

            query = OrderBySortPropertyAndDirection(query, pagingRequest.SortProperty, pagingRequest.Descending);

            var data = await query.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new
            {
                Data = data,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            } as IPage<T>;
        }
        public static IPage<T> Page<T>(this IQueryable<T> query, IPagingRequest pagingRequest)
        {
            var totalRecords = query.Count();

            var (pageSize, totalPages, pageNumber) = PageBase(pagingRequest, totalRecords);

            var data = query.Skip(pageNumber * pageSize).Take(pageSize).ToList();

            return new
            {
                Data = data,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            } as IPage<T>;
        }

        public static async Task<IPage<T>> PageAsync<T>(this IQueryable<T> query, IPagingRequest pagingRequest,
            CancellationToken cancellationToken)
        {
            var totalRecords = await query.CountAsync(cancellationToken);

            var (pageSize, totalPages, pageNumber) = PageBase(pagingRequest, totalRecords);

            var data = await query.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new
            {
                Data = data,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            } as IPage<T>;
        }

        private static (int, int, int) PageBase(IPagingRequest pagingRequest, int totalRecords)
        {
            //If no page size specified, show all
            var pageSize = pagingRequest.PageSize ?? int.MaxValue;

            var totalPages = Math.Max((int)Math.Ceiling(totalRecords / (double)pageSize), 1);

            var pageNumber = pagingRequest.PageNumber > totalPages ? 1 : pagingRequest.PageNumber;

            return (pageSize, totalPages, pageNumber);
        }
    }
}
