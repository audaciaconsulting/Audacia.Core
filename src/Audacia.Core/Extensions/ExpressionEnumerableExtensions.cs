using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Audacia.Core.Extensions
{
    /// <summary>
    /// Expression for an Enumerable of <see cref="Expression"/>s.
    /// </summary>
    public static class ExpressionEnumerableExtensions
    {
        /// <summary>
        /// Reduce multiple expressions with AND logic into a single expression.
        /// </summary>
        /// <param name="expressions">The list of expressions to combine.</param>
        /// <typeparam name="T">The type of the source of the predicate.</typeparam>
        /// <returns>A combined expression representing the OR logic of all input expressions.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expressions"/> is null.</exception>
        public static Expression<Func<T, bool>> Any<T>(
            this IEnumerable<Expression<Func<T, bool>>> expressions)
            where T : class
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            return expressions.Aggregate((left, right) => left.Or(right));
        }

        /// <summary>
        /// Reduce multiple expressions with OR logic into a single expression.
        /// </summary>
        /// <param name="expressions">The list of expressions to combine.</param>
        /// <typeparam name="T">The type of the source of the predicate.</typeparam>
        /// <returns>A combined expression representing the OR logic of all input expressions.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expressions"/> is null.</exception>
        public static Expression<Func<T, bool>> All<T>(
            this IEnumerable<Expression<Func<T, bool>>> expressions)
            where T : class
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            return expressions.Aggregate((left, right) => left.And(right));
        }
    }
}