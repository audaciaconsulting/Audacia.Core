using System;
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
        /// Adds the elements of the specified enumerable to the end of the collection.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            // Prevent add range on arrays
            if (destination.IsReadOnly)
            {
                throw new InvalidOperationException("Collection is read-only."); 
            }
            
            if (destination is List<T> list)
            {
                list.AddRange(source);
            }
            else foreach (var item in source)
            {
                destination.Add(item);
            }
        }

        /// <summary>
        /// Removes the elements of the specified enumerable from the collection.
        /// </summary>
        public static void RemoveARange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            // Prevent remove range on arrays
            if (destination.IsReadOnly)
            {
                throw new InvalidOperationException("Collection is read-only."); 
            }
            
            if (ReferenceEquals(destination, source))
            {
                destination.Clear();
            }
            else foreach (var item in source)
            {
                destination.Remove(item);
            }
        }
    }
}