using System;
using System.Linq;
using System.Linq.Expressions;

namespace Audacia.Core.Extensions
{
    /// <summary>
    /// Extension methods for the type <see cref="Queryable"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Append <see cref="OrderBy{T,TKey}"/> or <see cref="ThenBy{T,TKey}"/>,
        /// depending on whether the queryable has already been ordered.
        /// </summary>
        /// <typeparam name="T">The type of the collection.</typeparam>
        /// <typeparam name="TKey">The sort properties type.</typeparam>
        /// <param name="query">The original query.</param>
        /// <param name="keySelector">The sort property.</param>
        /// <param name="descending">Sort direction.</param>
        /// <returns>The <paramref name="query"/> ordered by <typeparamref name="TKey"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Design", "AV1130:Return type in method signature should be an interface to an unchangeable collection", Justification = "Extends and returns IQueryable for chaining.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?", Justification = "Easy to understand and implement.")]
        public static IOrderedQueryable<T> AppendOrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
            => (query?.Expression.Type ?? throw new ArgumentNullException(nameof(query), "Query can not be null")) == typeof(IOrderedQueryable<T>)
                    ? ((IOrderedQueryable<T>)query).ThenBy(keySelector, descending)
                    : query.OrderBy(keySelector, descending);

        /// <summary>
        /// Sort either ascending or descending, depending on the provided <paramref name="descending"/>.
        /// </summary>
        /// <typeparam name="T">The type of the collection.</typeparam>
        /// <typeparam name="TKey">The sort properties type.</typeparam>
        /// <param name="query">The original query.</param>
        /// <param name="keySelector">The sort property.</param>
        /// <param name="descending">Sort direction.</param>
        /// <returns>The <paramref name="query"/> ordered by <typeparamref name="TKey"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Design", "AV1130:Return type in method signature should be an interface to an unchangeable collection", Justification = "Returns IOrderedQueryable for chaining.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?", Justification = "Easy to understand and implement.")]
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
            => descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);

        /// <summary>
        /// Sort either ascending or descending, depending on the provided <paramref name="descending"/>.
        /// </summary>
        /// <typeparam name="T">The type of the collection.</typeparam>
        /// <typeparam name="TKey">The sort properties type.</typeparam>
        /// <param name="query">The original query.</param>
        /// <param name="keySelector">The sort property.</param>
        /// <param name="descending">Sort direction.</param>
        /// <returns>The <paramref name="query"/> ordered by <typeparamref name="TKey"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Design", "AV1130:Return type in method signature should be an interface to an unchangeable collection", Justification = "Returns IOrderedQueryable for chaining.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?", Justification = "Easy to understand and implement.")]
        public static IOrderedQueryable<T> ThenBy<T, TKey>(this IOrderedQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
            => descending
                ? query.ThenByDescending(keySelector)
                : query.ThenBy(keySelector);
    }
}