using System;

namespace Audacia.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the description attribute on an enumeration value, if there is no description attribute then returns the value to string
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type</typeparam>
        /// <param name="enumValue">The enumeration value</param>
        /// <returns>The description, falling back to ToString(), falling back to null if the supplied is not an enum</returns>
        public static string ToEnumDescriptionString<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            return typeof(TEnum).ToEnumDescriptionString(enumValue);
        }

        /// <summary>
        /// Returns the description attribute on an enumeration value, if there is no description attribute then returns the value to string
        /// </summary>
        /// <param name="type">The enumeration type</param>
        /// <param name="enumValue">The enumeration value</param>
        /// <returns>The description, falling back to ToString(), falling back to null if the supplied is not an enum</returns>
        public static string ToEnumDescriptionString(this Type type, object enumValue)
        {
            if (!type.IsEnum)
            {
                throw new NotSupportedException("An Enumeration type is required.");
            }

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            return fieldInfo != null ? fieldInfo.GetDataAnnotationDisplayName() : enumValue.ToString();
        }
    }
}