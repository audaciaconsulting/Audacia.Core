using System;

namespace Audacia.Core.Extensions;

/// <summary>
/// Extension methods for the type <see cref="Enum"/>.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Returns the description attribute on an enumeration value, if there is no description attribute then returns the value to string.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="enumValue">The enumeration value.</param>
    /// <returns>The description, falling back to ToString(), falling back to null if the supplied is not an enum.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1551:Method overload should call another overload", Justification = "Other 'overload' has different type parameters")]
    public static string? ToEnumDescriptionString<TEnum>(this TEnum enumValue) where TEnum : struct
    {
        return typeof(TEnum).ToEnumDescriptionString(enumValue);
    }

    /// <summary>
    /// Returns the description attribute on an enumeration value, if there is no description attribute then returns the value to string.
    /// </summary>
    /// <param name="type">The enumeration type.</param>
    /// <param name="enumValue">The enumeration value.</param>
    /// <returns>The description, falling back to ToString(), falling back to null if the supplied is not an enum.</returns>
    /// <exception cref="NotSupportedException">Type has to be enumerable.</exception>
    /// <exception cref="ArgumentNullException">All parameters are required and not nullable.</exception>
    public static string? ToEnumDescriptionString(this Type type, object enumValue)
    {
        ValidateEnumDescription(type, enumValue);

        var fieldInfo = enumValue.GetType().GetField(enumValue!.ToString() ?? string.Empty);

        return fieldInfo != null ? fieldInfo.GetDataAnnotationDisplayName() : enumValue?.ToString();
    }

    /// <summary>
    /// Attempts to parse a string to the equivalent constant value from the ennumeration type TEnum, using firstly
    /// the property name and then the description attribute if present. Outputs the parsed value and returns a 
    /// boolean value representing whether the action succeeded.
    /// </summary>
    /// <typeparam name="TEnum">The ennumeration type to which to parse the string.</typeparam>
    /// <param name="value">The string to convert.</param>
    /// <param name="result">The parsed value. If unsuccessful, the default value of the underlying TEnum.</param>
    /// <returns>A boolean value representing the success of the operation.</returns>
    public static bool TryParseDescription<TEnum>(this string value, out TEnum result)
        where TEnum : struct
    {
        if (Enum.TryParse(value, out result))
        {
            return true;
        }

        foreach (var enumValue in (TEnum[])Enum.GetValues(typeof(TEnum)))
        {
            if (string.Equals(enumValue.ToEnumDescriptionString(), value, StringComparison.OrdinalIgnoreCase))
            {
                result = enumValue;
                return true;
            }
        }

        return false;
    }

    private static void ValidateEnumDescription(Type type, object enumValue)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(enumValue);

        if (!type.IsEnum)
        {
            throw new NotSupportedException("An Enumeration type is required.");
        }
    }
}