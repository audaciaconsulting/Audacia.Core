using System.Collections.Generic;

namespace Audacia.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddIfNotContains<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }
        
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> range)
        {
            if (source is List<T> list)
            {
                list.AddRange(range);
            }
            else
            {
                foreach (var item in range)
                {
                    source.Add(item);
                }
            }
        }
    }
}