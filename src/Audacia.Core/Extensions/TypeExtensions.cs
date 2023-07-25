using System;
using System.Collections.Generic;

namespace Audacia.Core.Extensions;

/// <summary>
/// Extension methods for the type <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Type codes to check against in method <see cref="IsNumeric"/>.
    /// </summary>
    private static readonly List<TypeCode> NumberTypeCodes = new List<TypeCode>
    {
        TypeCode.Byte,
        TypeCode.SByte,
        TypeCode.UInt16,
        TypeCode.UInt32,
        TypeCode.UInt64,
        TypeCode.Int16,
        TypeCode.Int32,
        TypeCode.Int64,
        TypeCode.Decimal,
        TypeCode.Double,
        TypeCode.Single
    };

    /// <summary>
    /// Determine of specified type is nullable.
    /// </summary>
    /// <param name="this">The type to check if it is nullable.</param>
    /// <exception cref="ArgumentNullException"><paramref name="this"/> is null.</exception>
    /// <returns>If the type is nullable.</returns>
    public static bool IsNullable(this Type @this)
    {
        ArgumentNullException.ThrowIfNull(@this);

        return !@this.IsValueType || (@this.IsGenericType && @this.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// Return underlying type if type is nullable otherwise return the type.
    /// </summary>
    /// <param name="this">The type to check if it is nullable.</param>
    /// <returns>The none-nullable type.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="this"/> is null.</exception>
    public static Type GetUnderlyingTypeIfNullable(this Type @this)
    {
        ArgumentNullException.ThrowIfNull(@this);

        if (@this.IsNullable())
        {
            return !@this.IsValueType ? @this : Nullable.GetUnderlyingType(@this)!;
        }

        return @this;
    }

    /// <summary>
    /// Determines whether the underlying type is numeric.
    /// </summary>
    /// <param name="this">Property Type.</param> 
    /// <returns>true if the <see cref="TypeCode"/> is numeric.</returns>
    public static bool IsNumeric(this Type @this)
    {
        var underlyingType = @this.GetUnderlyingTypeIfNullable();
        var underlyingTypeCode = Type.GetTypeCode(underlyingType);
        return NumberTypeCodes.Contains(underlyingTypeCode);
    }
}