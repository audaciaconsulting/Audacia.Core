using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Audacia.Core.Extensions
{
    /// <summary>
    /// Extension methods for the type <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a dictionary with key = property name, value = value.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>Dictionary with key = property name, value = value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Design", "AV1130:Return type in method signature should be an interface to an unchangeable collection", Justification = "Type is limited to dictionaries.")]
        public static IDictionary<string, object?> ToPropertyDictionary(this object obj)
        {
            if (obj == null)
            {
                return new Dictionary<string, object?>();
            }

            return
                TypeDescriptor.GetProperties(obj.GetType())
                    .OfType<PropertyDescriptor>()
                    .ToDictionary(property => property.Name, property => property.GetValue(obj));
        }
    }
}