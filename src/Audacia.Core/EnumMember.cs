using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Audacia.Core.Extensions;

namespace Audacia.Core
{
    /// <summary>
    /// Provides a set of functions for converting enums to display name strings and back again.
    /// Supported attributes are;
    /// <see cref="DisplayNameAttribute"/>,
    /// <see cref="DescriptionAttribute"/>,
    /// <see cref="EnumMemberAttribute"/>.
    /// </summary>
    public static class EnumMember
    {
        /// <summary>
        /// Converts an enumerable of <see cref="{TEnum}"/> into an enumerable of display names.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="value">An enumerable of enum values</param>
        /// <returns>An enumerable of display names</returns>
        public static IEnumerable<string> AsEnumDisplayNames<TEnum>(this IEnumerable<TEnum> value) where TEnum : struct
        {
            ValidateValueObject(value);

            return value.Select(ToEnumDisplayName);
        }

        /// <summary>
        /// Converts a <see cref="{TEnum}"/> into a display name.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns>A human readable display name</returns>
        public static string ToEnumDisplayName<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            return GetDisplayName(enumValue) ??
                   GetDescription(enumValue) ??
                   GetEnumMemberValue(enumValue) ??
                   GetName(enumValue) ??
                   enumValue.ToString();
        }

        /// <summary>
        /// Returns the value set on the <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The description string</returns>
        public static string GetDescription(object enumValue)
        {
            ValidateValueObject(enumValue);

            var underlyingType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(underlyingType);

            return GetFieldInfo(enumValue)
                ?.GetCustomAttribute<DescriptionAttribute>(false)
                ?.Description;
        }

        /// <summary>
        /// Returns the value set on the <see cref="DisplayNameAttribute"/>.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The display name string</returns>
        public static string GetDisplayName(object enumValue)
        {
            ValidateValueObject(enumValue);

            var underlyingType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(underlyingType);

            return GetFieldInfo(enumValue)
                ?.GetCustomAttribute<DisplayNameAttribute>(false)
                ?.DisplayName;
        }

        /// <summary>
        /// Returns the value set on the <see cref="EnumMemberAttribute"/>.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The enummember value string</returns>
        public static string GetEnumMemberValue(object enumValue)
        {
            ValidateValueObject(enumValue);

            var underlyingType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(underlyingType);

            return GetFieldInfo(enumValue)
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value;
        }

        /// <summary>
        /// Returns the field name for the enum value.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The field name</returns>
        public static string GetName(object enumValue)
        {
            ValidateValueObject(enumValue);

            var underlyingType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(underlyingType);

            return GetFieldInfo(enumValue)?.Name;
        }

        /// <summary>
        /// Gets the value of an enum which corresponds to the specified enum member, display name, or description.
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="value">Input value</param>
        /// <exception cref="ArgumentNullException">enumType or value is null.</exception>
        /// <exception cref="ArgumentException">
        ///     enumType is not an System.Enum.
        ///     Or value is either an empty string, or only contains whitespace.
        ///     Or value is a name, but not one of the named constants defined for the enumeration.
        /// </exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <returns>The enum value as an object</returns>
        public static object Parse(Type enumType, string value)
        {
            var underlyingType = TypeExtensions.GetUnderlyingTypeIfNullable(enumType);
            ValidateEnumType(underlyingType);

            value = SanitizeDisplayNameString(value);
            ValidateValueString(value);

            object enumValue;
            bool enumDefined;

            try
            {
                // Match by number value or field name
                enumValue = Enum.Parse(underlyingType, value, ignoreCase: true);
                enumDefined = Enum.IsDefined(underlyingType, enumValue);
            }
            catch (ArgumentException)
            {
                // Ignore enum value not found error
                enumValue = null;
                enumDefined = false;
            }

            if (enumDefined)
            {
                // Enum was parsed successfully, and is in defined on the enum type
                return enumValue;
            }
            else if (enumValue != null)
            {
                throw new OverflowException($"Value '{value}' is outside the range of '${underlyingType.Name}'.");
            }

            // Get an enumerable of enum options
            var fields = underlyingType.GetFields().Where(f => f.IsStatic);

            // Attempt to match to each possible display value
            foreach (var field in fields)
            {
                if (MatchesEnumMember(field, value) ||
                    MatchesDisplayName(field, value) ||
                    MatchesDescription(field, value))
                {
                    return field.GetValue(null);
                }
            }

            throw new ArgumentException($"Requested value '{value}' was not found.");
        }

        /// <summary>
        /// Gets the value of an enum which corresponds to the specified enum member, display name, or description.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="value">Input value</param>
        /// <exception cref="ArgumentNullException">enumType or value is null.</exception>
        /// <exception cref="ArgumentException">
        ///     enumType is not an System.Enum.
        ///     Or value is either an empty string, or only contains whitespace.
        ///     Or value is a name, but not one of the named constants defined for the enumeration.
        /// </exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <returns>The enum value as an object</returns>
        public static TEnum Parse<TEnum>(string value) where TEnum : struct
        {
            return (TEnum)Parse(typeof(TEnum), value);
        }

        /// <summary>
        /// Gets the value of an enum which corresponds to the specified enum member, display name, or description.
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="value">Input value</param>
        /// <param name="enumValue">The enum value as an object</param>
        /// <returns><see langword="true" /> if parsed successfully</returns>
        public static bool TryParse(Type enumType, string value, out object enumValue) 
        {
            try
            {
                enumValue = Parse(enumType, value);
                return true;
            }
            catch 
            {
                enumValue = null;
            }

            return false;
        }

        /// <summary>
        /// Gets the value of an enum which corresponds to the specified enum member, display name, or description.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="value">Input value</param>
        /// <param name="enumValue">The enum value as an object</param>
        /// <returns><see langword="true" /> if parsed successfully</returns>
        public static bool TryParse<TEnum>(string value, out TEnum enumValue) where TEnum : struct
        {
            if (TryParse(typeof(TEnum), value, out var enumObj))
            {
                enumValue = (TEnum)enumObj;
                return true;
            }

            enumValue = default(TEnum);
            return false;
        }

        /// <summary>
        /// Gets all values for the provided enum type. 
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <returns>An array of enum values</returns>
        public static TEnum[] Values<TEnum>() where TEnum : struct
        {
            var enumType = typeof(TEnum);
            ValidateEnumType(enumType);

            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        private static FieldInfo GetFieldInfo(object enumValue)
        {
            return enumValue.GetType().GetField(enumValue.ToString());
        }

        private static bool MatchesDescription(MemberInfo member, string value)
        {
            var attribute = member.GetCustomAttribute<DescriptionAttribute>(false);

            if (attribute != null)
            {
                return string.Equals(attribute.Description, value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool MatchesDisplayName(MemberInfo member, string value)
        {
            var attribute = member.GetCustomAttribute<DisplayNameAttribute>(false);

            if (attribute != null)
            {
                return string.Equals(attribute.DisplayName, value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool MatchesEnumMember(MemberInfo member, string value)
        {
            var attribute = member.GetCustomAttribute<EnumMemberAttribute>(false);

            if (attribute != null)
            {
                return string.Equals(attribute.Value, value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string SanitizeDisplayNameString(string value)
        {
            // Ignore long dashes from user input
            return value?.Replace("–", "-").Trim();
        }

        private static void ValidateEnumType(Type enumType) 
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType), "Type cannot be null.");
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException(nameof(enumType), "Type must be an enum type.");
            }
        }

        private static void ValidateValueObject(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }
        }

        private static void ValidateValueString(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(nameof(value), "Value cannot be empty or whitespace.");
            }
        }
    }
}
