using System;

namespace Audacia.Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(this Type @this)
        {
            return !@this.IsValueType || (@this.IsGenericType && @this.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        /// <summary>
        /// Return underlying type if type is nullable otherwise return the type
        /// </summary>
        public static Type GetUnderlyingTypeIfNullable(this Type @this)
        {
            if (@this != null && @this.IsNullable())
            {
                return !@this.IsValueType ? @this : Nullable.GetUnderlyingType(@this);
            }

            return @this;
        }
    }
}