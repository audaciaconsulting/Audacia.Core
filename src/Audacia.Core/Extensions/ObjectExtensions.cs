using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Audacia.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a dictionary with key = property name, value = value.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>Dictionary with key = property name, value = value.</returns>
        public static IDictionary<string, object> ToPropertyDictionary(this object obj)
        {
            return
                TypeDescriptor.GetProperties(obj.GetType())
                    .OfType<PropertyDescriptor>()
                    .ToDictionary(property => property.Name, property => property.GetValue(obj));
        }
    }
}