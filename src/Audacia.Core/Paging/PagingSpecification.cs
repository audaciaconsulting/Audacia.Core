using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Audacia.Core.Extensions;

namespace Audacia.Core
{
    /// <summary>
    /// Specification for how we'll page a queryable of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The return type of the query.</typeparam>
    public class PagingSpecification<T>
    {
        /// <summary>
        /// Gets the underlying query for the data to be paged.
        /// </summary>
        public IQueryable<T> Query { get; private set; }

        private int _pageNumber;
        private int _pageSize = int.MaxValue;
        private static readonly Type Type = typeof(T);
        private string _sortProperty = string.Empty;
        private bool _descending = false;

        /// <summary>
        /// Construct a specification for how we'll page a queryable of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="query">The query to implement a specification.</param>
        public PagingSpecification(IQueryable<T> query)
        {
            Query = query;
        }

        /// <summary>
        /// Record paging information for use when getting the page of results.
        /// </summary>
        /// <param name="pagingRequest">Contains the information required for paging.</param>
        /// <returns>A paging specifications with the paging provided from <paramref name="pagingRequest"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagingRequest"/> is null.</exception>
        public PagingSpecification<T> ConfigurePaging(PagingRequest pagingRequest)
        {
            if (pagingRequest == null)
            {
                throw new ArgumentNullException(nameof(pagingRequest));
            }

            return ConfigurePaging(pagingRequest.PageSize, pagingRequest.PageNumber);
        }

        /// <summary>
        /// Record paging information for use when getting the page of results.
        /// </summary>
        /// <param name="pageSize">The number of results to show per page.</param>
        /// <param name="pageNumber">Which page of results we want to show. The first page is 1.</param>
        /// <returns>A paging specifications with the <paramref name="pageSize"/> and <paramref name="pageNumber"/>.</returns>
        public virtual PagingSpecification<T> ConfigurePaging(int? pageSize, int pageNumber)
        {
            _pageSize = pageSize ?? int.MaxValue;
            _pageNumber = pageNumber;
            return this;
        }

        /// <summary>
        /// Record sorting information for use when sorting the results.
        /// </summary>
        /// <param name="pagingRequest">Contains the information required for sorting.</param>
        /// <returns>A paging specifications with the sorting provided from <paramref name="pagingRequest"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pagingRequest"/> is null.</exception>
        public PagingSpecification<T> ConfigureSorting(SortablePagingRequest pagingRequest)
        {
            if (pagingRequest == null)
            {
                throw new ArgumentNullException(nameof(pagingRequest));
            }

            return ConfigureSorting(pagingRequest.SortProperty, pagingRequest.Descending);
        }

        /// <summary>
        /// Record sorting information for use when sorting the results.
        /// </summary>
        /// <param name="sortProperty">The property of <typeparamref name="T"/> we want to sort.</param>
        /// <param name="descending">The sort direction.</param>
        /// <returns>A paging specifications with the <paramref name="sortProperty"/> and <paramref name="descending"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?", Justification = "Makes sense when passed from UI.")]
        public virtual PagingSpecification<T> ConfigureSorting(string sortProperty, bool descending)
        {
            _sortProperty = sortProperty;
            _descending = descending;
            return this;
        }

        /// <summary>
        /// Sort the query based on the configured sorting information.
        /// </summary>
        /// <exception cref="ArgumentException">If our configured sortProperty is invalid.</exception>
        /// <returns>Paging specification with sorting enabled.</returns>
        public PagingSpecification<T> UseSorting()
        {
            SortQuery();
            return this;
        }

        /// <summary>
        /// Update the Query to return a page of results.
        /// </summary>
        /// <returns>Paging specification with paging enabled.</returns>
        public PagingSpecification<T> UsePaging()
        {
            Query = Query.Skip(_pageNumber * _pageSize).Take(_pageSize);
            return this;
        }

        /// <summary>
        /// Get the number of pages the results take up, based on the provided <paramref name="totalRecords"/>.
        /// </summary>
        /// <param name="totalRecords">The total number of records.</param>
        /// <returns>The total number of pages possible with the provided <paramref name="totalRecords"/> count.</returns>
        public int GetTotalPages(int totalRecords)
        {
            var totalPages = Math.Max((int)Math.Ceiling(totalRecords / (double)_pageSize), 1);
            // Set the page number to 1 if we've asked for a larger page than we have. 
            _pageNumber = _pageNumber > totalPages ? 1 : _pageNumber;
            return totalPages;
        }

        private void SortQuery()
        {
            if (string.IsNullOrWhiteSpace(_sortProperty))
            {
                return;
            }

            //Upper case first to account for lower case JSON
            var propertyInfo = Type.GetProperty(_sortProperty.UpperCaseFirst());

            if (propertyInfo == null)
            {
                // Someone has provided a sort property that doesn't exist on the result
                throw new ArgumentException($"Invalid Sort Property: {_sortProperty}");
            }

            var orderByExpression = GetOrderByExpression(propertyInfo);

            var orderMethod = _descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

            var method =
                typeof(Queryable).GetMethods().First(m => m.Name == orderMethod && m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(Type, propertyInfo.PropertyType);

            Query = (genericMethod.Invoke(null, new object[] { Query, orderByExpression }) as IQueryable<T>) ?? default!;
        }

        private static LambdaExpression GetOrderByExpression(MemberInfo propertyInfo)
        {
            var parameterExpression = Expression.Parameter(Type);
            var propertyExpression = Expression.PropertyOrField(parameterExpression, propertyInfo.Name);

            return Expression.Lambda(propertyExpression, parameterExpression);
        }
    }
}