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
    }
}