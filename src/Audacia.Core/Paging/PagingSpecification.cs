using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Audacia.Core.Extensions;

namespace Audacia.Core
{
    /// <summary>
    /// Specification for how we'll page a queryable of <typeparamref name="T"/>.
    /// </summary>
    public class PagingSpecification<T>
    {
        /// <summary>
        /// The underlying query for the data to be paged.
        /// </summary>
        public IQueryable<T> Query { get; private set; }

        private int _pageNumber;
        private int _pageSize = int.MaxValue;
        private static Type Type { get; } = typeof(T);

        public PagingSpecification(IQueryable<T> query)
        {
            Query = query;
        }

        public PagingSpecification<T> WithPaging(PagingRequest pagingRequest)
        {
            return WithPaging(pagingRequest.PageSize, pagingRequest.PageNumber);
        }

        public PagingSpecification<T> WithPaging(int? pageSize, int pageNumber)
        {
            _pageSize = pageSize ?? int.MaxValue;
            _pageNumber = pageNumber;
            return this;
        }

        public PagingSpecification<T> ApplySorting(SortablePagingRequest pagingRequest)
        {
            SortQuery(pagingRequest.SortProperty, pagingRequest.Descending);
            return this;
        }

        /// <param name="sortProperty">The property of <typeparamref name="T"/> we'll sort by</param>
        /// <param name="descending">The sort direction</param>
        /// <exception cref="ArgumentException">If the provided <paramref name="sortProperty"/> is invalid.</exception>
        public PagingSpecification<T> ApplySorting(string sortProperty, bool descending)
        {
            SortQuery(sortProperty, descending);
            return this;
        }

        /// <summary>
        /// Update the Query to return a page of results
        /// </summary>
        public PagingSpecification<T> ApplyPaging()
        {
            Query = Query.Skip(_pageNumber * _pageSize).Take(_pageSize);
            return this;
        }

        public int GetTotalPages(int totalRecords)
        {
            var totalPages = Math.Max((int) Math.Ceiling(totalRecords / (double) _pageSize), 1);
            // Set the page number to 1 if we've asked for a larger page than we have. 
            _pageNumber = _pageNumber > totalPages ? 1 : _pageNumber;
            return totalPages;
        }

        private void SortQuery(string sortProperty, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortProperty))
            {
                return;
            }

            //Upper case first to account for lower case JSON
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

            Query = genericMethod.Invoke(null, new object[] {Query, orderByExpression}) as IQueryable<T>;
        }

        private static LambdaExpression GetOrderByExpression(MemberInfo propertyInfo)
        {
            var parameterExpression = Expression.Parameter(Type);
            var propertyExpression = Expression.PropertyOrField(parameterExpression, propertyInfo.Name);

            return Expression.Lambda(propertyExpression, parameterExpression);
        }
    }
}