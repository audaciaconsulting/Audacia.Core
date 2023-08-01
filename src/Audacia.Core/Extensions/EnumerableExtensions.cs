using System.Collections.Generic;
using System.Linq;

namespace Audacia.Core.Extensions
{
    /// <summary>
    /// Extension methods for the type <see cref="Enumerable"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Groups a collection by a the <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type to group by.</typeparam>
        /// <param name="enumerable">The collection to group.</param>
        /// <param name="number">The number of groups.</param>
        /// <returns>A <see cref="IEnumerable{T}"/>, grouped by <typeparamref name="T"/>.</returns>
        public static IEnumerable<IGrouping<int, T>> GroupsOf<T>(this IEnumerable<T> enumerable, int number)
        {
            return enumerable.Select((item, index) => new { item, index })
                .GroupBy(entry => entry.index / number, entry => entry.item)
                .ToList();
        }

        /// <summary>
        /// Converts a grouped collection to a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The group key.</typeparam>
        /// <typeparam name="TValue">The group values.</typeparam>
        /// <param name="grouping">The grouped collection.</param>
        /// <returns>A dictionary, whose key is the groups key, and values are the values for that key.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Design", "AV1130:Return type in method signature should be an interface to an unchangeable collection", Justification = "Limited to dictionaries.")]
        public static IDictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(
            this IEnumerable<IGrouping<TKey, TValue>> grouping) where TKey : notnull
        {
            return grouping.ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}