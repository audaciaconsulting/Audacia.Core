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

        /// <summary>
        /// Attempts to parse a string to the equivalent constant value from the ennumeration type TEnum, using firstly
        /// the property name and then the description attribute if present. Outputs the parsed value and returns a 
        /// boolean value representing whether the action succeeded
        /// </summary>
        /// <typeparam name="TEnum">The ennumeration type to which to parse the string</typeparam>
        /// <param name="value">The string to convert</param>
        /// <param name="result">The parsed value. If unsuccessful, the default value of the underlying TEnum</param>
        /// <returns>A boolean value representing the success of the operation</returns>
        public static bool TryParseDescription<TEnum>(this string value, out TEnum result)
            where TEnum : struct
        {
            if (Enum.TryParse(value, out result))
            {
                return true;
            }

            foreach (var enumValue in (TEnum[])Enum.GetValues(typeof(TEnum)))
            {
                if (string.Equals(enumValue.ToEnumDescriptionString(), value, 
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    result = enumValue;
                    return true;
                }
            }

            return false;
        }
    }
}