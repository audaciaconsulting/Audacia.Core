using System;
using System.Linq;
using System.Linq.Expressions;

namespace Audacia.Core.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Append <see cref="OrderBy{T,TKey}"/> or <see cref="ThenBy{T,TKey}"/>,
        /// depending on whether the queryable has already been ordered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query"></param>
        /// <param name="keySelector"></param>
        /// <param name="descending">Sort direction.</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> AppendOrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
            => query.Expression.Type == typeof(IOrderedQueryable<T>)
                ? ((IOrderedQueryable<T>)query).ThenBy(keySelector, descending)
                : query.OrderBy(keySelector, descending);


        /// <summary>
        /// Sort either ascending or descending, depending on the provided <see cref="descending"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">Type of the property you're ordering by.</typeparam>
        /// <param name="source"></param>
        /// <param name="propertyGetter"></param>
        /// <param name="descending">Sort direction.</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> propertyGetter, bool descending)
            => descending
                ? source.OrderByDescending(propertyGetter)
                : source.OrderBy(propertyGetter);


        /// <summary>
        /// Sort either ascending or descending, depending on the provided <see cref="descending"/>. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">Type of the property you're ordering by.</typeparam>
        /// <param name="source"></param>
        /// <param name="propertyGetter"></param>
        /// <param name="descending">Sort direction.</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T, TKey>(this IOrderedQueryable<T> source, Expression<Func<T, TKey>> propertyGetter, bool descending)
            => descending
                ? source.ThenByDescending(propertyGetter)
                : source.ThenBy(propertyGetter);
    }
}