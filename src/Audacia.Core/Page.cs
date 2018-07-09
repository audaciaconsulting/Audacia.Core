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
        public int TotalPages { get; }
        public int TotalRecords { get; }
        public IEnumerable<T> Data { get; }

        public Page(int totalPages, int totalRecords, IEnumerable<T> data)
        {
            TotalPages = totalPages;
            TotalRecords = totalRecords;
            Data = data;
        }

        public Page(IQueryable<T> query, ISortablePagingRequest sortablePagingRequest)
        {
            TotalRecords = query.Count();

            var pagingInfo = PageBase(sortablePagingRequest, TotalRecords);

            TotalPages = pagingInfo.TotalPages;

            var sortProperty = sortablePagingRequest.SortProperty;
            var descending = sortablePagingRequest.Descending;

            query = OrderBySortPropertyAndDirection(query, sortProperty, descending);

            Data = query.Skip(pagingInfo.PageNumber * pagingInfo.PageSize).Take(pagingInfo.PageSize).ToList();
        }

        public Page(IEnumerable<T> enumerable, ISortablePagingRequest sortablePagingRequest)
            : this(enumerable.AsQueryable(), sortablePagingRequest)
        {
        }
        
        public static PagingInfo PageBase(IPagingRequest pagingRequest, int totalRecords)
        {
            //If no page size specified, show all
            var pageSize = pagingRequest.PageSize ?? int.MaxValue;

            var totalPages = Math.Max((int)Math.Ceiling(totalRecords / (double)pageSize), 1);

            var pageNumber = pagingRequest.PageNumber > totalPages ? 1 : pagingRequest.PageNumber;

            return new PagingInfo
            {
                PageSize = pageSize,
                TotalPages = totalPages,
                PageNumber = pageNumber
            };
        }
        
        public static IOrderedQueryable<T> OrderBySortPropertyAndDirection(IQueryable<T> query, string sortProperty,
            bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortProperty))
            {
                throw new ArgumentException("No Sort Property provided", nameof(sortProperty));
            }

            var queryType = typeof(T);
            //Upper case first to account from lower case JSON
            var propertyInfo = queryType.GetProperty(sortProperty.UpperCaseFirst());

            if (propertyInfo == null)
            {
                // Someone has provided a sort property that doesn't exist on the result
                throw new ArgumentException("Invalid Sort Property", nameof(sortProperty));
            }

            var orderByExpression = GetOrderByExpression(propertyInfo, queryType);

            var orderMethod = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

            var method =
                typeof(Queryable).GetMethods().First(m => m.Name == orderMethod && m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(queryType, propertyInfo.PropertyType);

            return genericMethod.Invoke(null, new object[] { query, orderByExpression }) as IOrderedQueryable<T>;
        }

        private static LambdaExpression GetOrderByExpression(MemberInfo propertyInfo, Type queryType)
        {
            var parameterExpression = Expression.Parameter(queryType);
            var propertyExpression = Expression.PropertyOrField(parameterExpression, propertyInfo.Name);

            return Expression.Lambda(propertyExpression, parameterExpression);
        }
    }
}