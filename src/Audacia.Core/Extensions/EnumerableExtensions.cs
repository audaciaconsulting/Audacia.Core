using System.Collections.Generic;
using System.Linq;

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
        /// Converts a grouped collection to a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The group key.</typeparam>
        /// <typeparam name="TValue">The group values.</typeparam>
        /// <param name="grouping">The grouped collection.</param>
        /// <returns>A dictionary, whose key is the groups key, and values are the values for that key.</returns>
        public static IDictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(
            this IEnumerable<IGrouping<TKey, TValue>> grouping)
        {
            return grouping.ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}