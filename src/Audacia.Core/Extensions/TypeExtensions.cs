using System;
using System.Collections.Generic;

namespace Audacia.Core.Extensions
{
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
        public static bool IsNullable(this Type @this)
        {
            return !@this.IsValueType || (@this.IsGenericType && @this.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        /// <summary>
        /// Return underlying type if type is nullable otherwise return the type.
        /// </summary>
        public static Type GetUnderlyingTypeIfNullable(this Type @this)
        {
            if (@this != null && @this.IsNullable())
            {
                return !@this.IsValueType ? @this : Nullable.GetUnderlyingType(@this);
            }

            return @this;
        }

        /// <summary>
        /// Determines whether the underlying type is numeric.
        /// </summary>
        /// <param name="t">Property Type.</param> 
        /// <returns>true if the <see cref="TypeCode"/> is numeric.</returns>
        public static bool IsNumeric(this Type t)
        {
            var underlyingType = t.GetUnderlyingTypeIfNullable();
            var underlyingTypeCode = Type.GetTypeCode(underlyingType);
            return NumberTypeCodes.Contains(underlyingTypeCode);
        }
    }
}