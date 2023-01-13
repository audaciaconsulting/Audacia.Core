using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Audacia.Core.Extensions;

namespace Audacia.Core
{
    /// <summary>
    /// Provides a set of functions for converting enums to display name strings and back again.
    /// Supported attributes are;
    /// <see cref="DescriptionAttribute"/>,
    /// <see cref="EnumMemberAttribute"/>.
    /// </summary>
    public static class EnumMember
    {
        /// <summary>
        /// Retrieves the description for all fields on the provided enum type.
        /// For fields without a <see cref="DescriptionAttribute"/>, <see langword="null"/> will be returned.
        /// </summary>
        /// <param name="enumType">Enum type</param>
        public static IEnumerable<string> Descriptions(Type enumType)
        {
            ValidateEnumType(enumType);

            foreach (var value in Enum.GetValues(enumType))
            {
                yield return GetDescription(value);
            }
        }

        /// <summary>
        /// Retrieves the description for all fields on the provided enum type.
        /// For fields without a <see cref="DescriptionAttribute"/>, <see langword="null"/> will be returned.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        public static IEnumerable<string> Descriptions<TEnum>() where TEnum : struct
        {
            return Descriptions(typeof(TEnum));
        }

        /// <summary>
        /// Retrieves the enum member value for all fields on the provided enum type.
        /// For fields without a <see cref="EnumMemberAttribute"/>, <see langword="null"/> will be returned.
        /// </summary>
        /// <param name="enumType">Enum type</param>
        public static IEnumerable<string> EnumMemberValues(Type enumType)
        {
            ValidateEnumType(enumType);

            foreach (var value in Enum.GetValues(enumType))
            {
                yield return GetEnumMemberValue(value);
            }
        }

        /// <summary>
        /// Retrieves the enum member value for all fields on the provided enum type.
        /// For fields without a <see cref="EnumMemberAttribute"/>, <see langword="null"/> will be returned.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        public static IEnumerable<string> EnumMemberValues<TEnum>() where TEnum : struct
        {
            return EnumMemberValues(typeof(TEnum));
        }

        /// <summary>
        /// Retrieves the field name for all fields on the provided enum type.
        /// </summary>
        /// <param name="enumType">Enum type</param>
        public static IEnumerable<string> Names(Type enumType)
        {
            ValidateEnumType(enumType);

            foreach (var value in Enum.GetValues(enumType))
            {
                yield return GetName(value);
            }
        }

        /// <summary>
        /// Retrieves the field name for all fields on the provided enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        public static IEnumerable<string> Names<TEnum>() where TEnum : struct
        {
            return Names(typeof(TEnum));
        }

        /// <summary>
        /// Retrieves the first available human readable name for all fields in the provided enum type.
        /// </summary>
        /// <param name="enumType">Enum type</param>
        public static IEnumerable<string> Options(Type enumType)
        {
            ValidateEnumType(enumType);

            foreach (var value in Enum.GetValues(enumType))
            {
                yield return GetOption(value);
            }
        }

        /// <summary>
        /// Retrieves the first available human readable name for all fields in the provided enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        public static IEnumerable<string> Options<TEnum>() where TEnum : struct
        {
            return Options(typeof(TEnum));
        }

        /// <summary>
        /// Returns the value set on the <see cref="DescriptionAttribute"/>.
        /// For fields without a <see cref="DescriptionAttribute"/>, <see langword="null"/> will be returned.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The description string</returns>
        public static string GetDescription(object enumValue)
        {
            ValidateValueObject(enumValue);

            var enumType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(enumType);

            return GetFieldInfo(enumValue)
                ?.GetCustomAttribute<DescriptionAttribute>(false)
                ?.Description;
        }

        /// <summary>
        /// Returns the value set on the <see cref="EnumMemberAttribute"/>.
        /// For fields without a <see cref="EnumMemberAttribute"/>, <see langword="null"/> will be returned.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The enummember value string</returns>
        public static string GetEnumMemberValue(object enumValue)
        {
            ValidateValueObject(enumValue);

            var enumType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(enumType);

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

            var enumType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(enumType);

            return GetFieldInfo(enumValue)?.Name;
        }

        /// <summary>
        /// Returns the first available human readable name for the enum value.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        public static string GetOption(object enumValue)
        {
            return GetDescription(enumValue) ??
                   GetEnumMemberValue(enumValue) ??
                   GetName(enumValue) ??
                   enumValue?.ToString();
        }

        /// <summary>
        /// Returns the enum value as a text integer.
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The field name</returns>
        public static string GetValue(object enumValue)
        {
            ValidateValueObject(enumValue);

            var enumType = TypeExtensions.GetUnderlyingTypeIfNullable(enumValue.GetType());

            ValidateEnumType(enumType);

            return Convert.ToInt32(enumValue).ToString();
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
            ValidateEnumType(enumType);

            value = SanitizeDisplayNameString(value);
            ValidateValueString(value);

            object enumValue;
            bool enumDefined;

            try
            {
                // Match by number value or field name
                enumValue = Enum.Parse(enumType, value, ignoreCase: true);
                enumDefined = Enum.IsDefined(enumType, enumValue);
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
                throw new OverflowException($"Value '{value}' is outside the range of '${enumType.Name}'.");
            }

            // Get an enumerable of enum options
            var fields = enumType.GetFields().Where(f => f.IsStatic);

            // Attempt to match to each possible display value
            foreach (var field in fields)
            {
                if (MatchesEnumMember(field, value) ||
                    MatchesDescription(field, value) ||
                    MatchesName(field, value) ||
                    MatchesDisplay(field,value) ||
                    MatchesDisplayName(field,value))
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
        /// Retrieves the value for all fields on the provided enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <returns>An integer array of enum values</returns>
        public static IEnumerable<int> Values(Type enumType)
        {
            ValidateEnumType(enumType);

            return (int[])Enum.GetValues(enumType);
        }

        /// <summary>
        /// Retrieves the value for all fields on the provided enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <returns>An array of enum values</returns>
        public static IEnumerable<TEnum> Values<TEnum>() where TEnum : struct
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

        private static bool MatchesEnumMember(MemberInfo member, string value)
        {
            var attribute = member.GetCustomAttribute<EnumMemberAttribute>(false);

            if (attribute != null)
            {
                return string.Equals(attribute.Value, value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool MatchesDisplay(MemberInfo member, string value)
        {
            var attribute = member.GetCustomAttribute<DisplayAttribute>(false);

            if (attribute != null)
            {
                return string.Equals(attribute.Name, value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool MatchesDisplayName(MemberInfo member, string value)
        {
            var attribute = member.GetCustomAttribute<DisplayNameAttribute>(false);

            if(attribute != null)
            {
                return string.Equals(attribute.DisplayName, value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool MatchesName(MemberInfo member, string value)
        {
            return string.Equals(member.Name, value, StringComparison.OrdinalIgnoreCase);
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
