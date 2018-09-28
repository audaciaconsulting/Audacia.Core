using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Audacia.Core.Extensions;

namespace Audacia.Core
{
    public class SortablePagingRequest<T> : SortablePagingRequest
    {
        public SortablePagingRequest()
            : this(int.MaxValue, 0)
        {
            /* Required for injection */
        }

        public SortablePagingRequest(int pageSize = int.MaxValue, int pageNumber = 0)
            : base(pageSize, pageNumber)
        {
        }

        /// <summary>
        /// The properties you want to sort by, in the order they should be applied
        /// </summary>
        public List<Expression<Func<T, object>>> SortExpressions { get; set; } = new List<Expression<Func<T, object>>>();

        public void SetSortExpression(string propertyName)
        {
            var expression = propertyName.ToLambdaExpression<T>();
            this.SortExpressions.Add(expression);
        }
    }
    
    public class SortablePagingRequest: PagingRequest
    {
        public SortablePagingRequest()
            : this(int.MaxValue, 0)
        {
            /* Required for injection */
        }
        public SortablePagingRequest(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public bool Descending { get; set; }

        public string SortProperty { get; set; }
    }
}