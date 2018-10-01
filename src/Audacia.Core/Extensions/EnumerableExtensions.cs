using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Audacia.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IGrouping<int, T>> GroupsOf<T>(this IEnumerable<T> enumerable, int number)
        {
            return enumerable.Select((item, index) => new { item, index })
                .GroupBy(entry => entry.index / number, entry => entry.item);
        }

        /// <summary>
        /// Converts a grouped collection to a dictionary
        /// </summary>
        /// <typeparam name="TKey">The group key</typeparam>
        /// <typeparam name="TValue">The group values</typeparam>
        /// <param name="grouping">The grouped collection</param>
        /// <returns>A dictionary, whose key is the groups key, and values are the values for that key</returns>
        public static IDictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(
            this IEnumerable<IGrouping<TKey, TValue>> grouping)
        {
            return grouping.ToDictionary(group => group.Key, group => group.ToList());
        }

        /// <summary>
        /// Append <see cref="OrderBy{T,TKey}"/> or <see cref="ThenBy{T,TKey}"/>,
        /// depending on whether the queryable has alraedy been ordered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="query"></param>
        /// <param name="keySelector"></param>
        /// <param name="descending">Sort direction</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> AppendOrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
            => query.Expression.Type == typeof(IOrderedQueryable<T>)
                ? ((IOrderedQueryable<T>)query).ThenBy(keySelector, descending)
                : query.OrderBy(keySelector, descending);


        /// <summary>
        /// Sort either ascending or descending, depending on the provided <see cref="descending"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">Type of the property you're ordering by</typeparam>
        /// <param name="source"></param>
        /// <param name="propertyGetter"></param>
        /// <param name="descending">Sort direction</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> propertyGetter, bool descending)
            => descending
                ? source.OrderByDescending(propertyGetter)
                : source.OrderBy(propertyGetter);


        /// <summary>
        /// Sort either ascending or descending, depending on the provided <see cref="descending"/>. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">Type of the property you're ordering by</typeparam>
        /// <param name="source"></param>
        /// <param name="propertyGetter"></param>
        /// <param name="descending">Sort direction</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T, TKey>(this IOrderedQueryable<T> source, Expression<Func<T, TKey>> propertyGetter, bool descending)
            => descending
                ? source.ThenByDescending(propertyGetter)
                : source.ThenBy(propertyGetter);

    }
}