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
        public IEnumerable<T> Data { get; }

        private Type Type { get; } = typeof(T);

        public Page(IQueryable<T> query, SortablePagingRequest sortablePagingRequest)
        {
            var (pageNumber, pageSize) = PageBase(query, sortablePagingRequest);

            var sortProperty = sortablePagingRequest.SortProperty;
            var descending = sortablePagingRequest.Descending;

            query = OrderBySortPropertyAndDirection(query, sortProperty, descending);

            Data =
                query.Skip(pageNumber * pageSize).Take(pageSize).ToList();
        }

        public Page(IEnumerable<T> enumerable, SortablePagingRequest sortablePagingRequest)
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

            //If no page size specified, show all
            var pageSize = pagingRequest.PageSize ?? int.MaxValue;

            TotalPages = Math.Max((int)Math.Ceiling(TotalRecords / (double)pageSize), 1);

            var pageNumber = pagingRequest.PageNumber > TotalPages ? 1 : pagingRequest.PageNumber;

            return (pageNumber, pageSize);
        }

        public static IQueryable<T> OrderBySortPropertyAndDirection(IQueryable<T> query, string sortProperty,
            bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortProperty))
            {
                return query;
            }

            //Upper case first to account from lower case JSON
            var propertyInfo = Type.GetProperty(sortProperty.UpperCaseFirst());

            if (propertyInfo == null)
            {
                // Someone has provided a sort property that doesn't exist on the result
                throw new ArgumentException("Invalid Sort Property", nameof(sortProperty));
            }

            var orderByExpression = GetOrderByExpression(propertyInfo);

            var orderMethod = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

            var method =
                typeof(Queryable).GetMethods().First(m => m.Name == orderMethod && m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(Type, propertyInfo.PropertyType);

            return genericMethod.Invoke(null, new object[] { query, orderByExpression }) as IQueryable<T>;
        }

        private static LambdaExpression GetOrderByExpression(MemberInfo propertyInfo)
        {
            var parameterExpression = Expression.Parameter(Type);
            var propertyExpression = Expression.PropertyOrField(parameterExpression, propertyInfo.Name);

            return Expression.Lambda(propertyExpression, parameterExpression);
        }
    }
}