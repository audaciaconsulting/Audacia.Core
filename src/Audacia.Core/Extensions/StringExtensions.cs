using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Audacia.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes invalid file name characters from a string to resturn a string than can be used as part of a filename or directory structure
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The output string with unsafe characters removed</returns>
        public static string ToSafeFilename(this string input)
        {
            var invalidCharacters = Path.GetInvalidFileNameChars();

            return new string(input.Trim().Where(c => !invalidCharacters.Contains(c)).ToArray());
        }

        /// <summary>
        /// Attempts to convert any string to the requested type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="input">The string to convert</param>
        /// <param name="output">The converted result (or default if convert fails)</param>
        /// <param name="invariant">Whether to use culture invariance in the conversion</param>
        /// <returns>Whether the conversion passed or failed</returns>
        public static bool TryConvertToType<T>(this string input, out T output, bool invariant = false)
        {
            try
            {
                output = input.ConvertToType<T>(invariant);
                return true;
            }
            catch (Exception)
            {
                output = default(T);
                return false;
            }
        }

        /// <summary>
        /// Adds spaces before capitals in a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The transformed string</returns>
        public static string AddSpacesBeforeCapitals(this string source)
        {
            return Regex.Replace(source, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }

        /// <summary>
        /// Converts any string to the requested type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="input">The string to convert</param>
        /// <param name="invariant">Whether to use culture invariance in the conversion</param>
        /// <returns>The converted string</returns>
        public static T ConvertToType<T>(this string input, bool invariant = false)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)(invariant ? typeConverter.ConvertFromInvariantString(input) : typeConverter.ConvertFromString(input));
        }

        /// <summary>
        /// Replace any instances of the dictionary keys with the dictionary values in the requested string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="findReplace">The find and replace pairs</param>
        /// <returns>The resultant string</returns>
        public static string FindAndReplace(this string source, IDictionary<string, string> findReplace)
        {
            var stringBuilder = new StringBuilder(source);

            foreach (var item in findReplace)
            {
                stringBuilder.Replace(item.Key, item.Value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Takes a substring from the end of a string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <param name="tailLength">The length of the substring</param>
        /// <returns>The substring</returns>
        public static string SubstringFromEnd(this string input, int tailLength)
        {
            return tailLength >= input.Length ? input : input.Substring(input.Length - tailLength);
        }

        /// <summary>
        /// Converts a string to title case
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The converted string</returns>
        public static string ToTitleCase(this string source)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source);
        }

        /// <summary>
        /// Removes non numeric values from a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The string stripped of everything except numerals, full stops and negative figures</returns>
        public static string RemoveNonNumeric(this string source)
        {
            return string.IsNullOrWhiteSpace(source)
                ? string.Empty
                : new string(source.ToCharArray().Where(e => char.IsDigit(e) || e == '.' || e == '-').ToArray());
        }

        /// <summary>
        /// Removes numeric values from a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>The string stripped of numerals</returns>
        public static string RemoveNumeric(this string source)
        {
            return string.IsNullOrWhiteSpace(source)
                ? string.Empty
                : new string(source.ToCharArray().Where(e => !char.IsDigit(e)).ToArray());
        }

        /// <summary>
        /// Determines whether the contents of a string are numeric
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The result</returns>
        public static bool IsNumeric(this string input)
        {
            return !string.IsNullOrWhiteSpace(input) &&
                   input.ToCharArray().All(e => char.IsDigit(e) || e == '.' || e == '-');
        }

        /// <summary>
        /// Determines whether a string contains numerals
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The result</returns>
        public static bool HasNumeric(this string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.ToCharArray().Any(char.IsDigit);
        }

        /// <summary>
        /// Trims all instances of the requested string from the start of a string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="trimmable">The string to strip from the start</param>
        /// <returns>The stripped string</returns>
        public static string TrimStart(this string source, string trimmable)
        {
            while (source.StartsWith(trimmable))
            {
                source = source.Substring(trimmable.Length);
            }

            return source;
        }

        /// <summary>
        /// Truncates a string by a specified number of characters, and includes an appended string if it exceeds a specified limits.
        /// NOTE: The amount of characters taken if the string exceeds the character limit is: (CharacterLimit - appendString.length).
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="characterLimit">The amount of characters to accept before truncating</param>
        /// <param name="appendStringIfLimitExceeded">Some characters to append if limit hit</param>
        /// <returns></returns>
        public static string Truncate(this string source, int characterLimit, string appendStringIfLimitExceeded = "...")
        {
            if (source.Length <= characterLimit)
            {
                return source;
            }

            return (source.Substring(0, characterLimit - appendStringIfLimitExceeded.Length)).Trim() + appendStringIfLimitExceeded;
        }

        /// <summary>
        /// Split a camel case string into separate words
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The split string</returns>
        public static string SplitCamelCase(this string source)
        {
            return Regex.Replace(source, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Lowercase the first character of a string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The transformed string</returns>
        public static string LowerCaseFirst(this string input)
        {
            return input.Any() ? char.ToLowerInvariant(input[0]) + input.Substring(1) : input;
        }

        /// <summary>
        /// Uppercase the first character of a string
        /// </summary>
        /// <param name="input">The source string</param>
        /// <returns>The transformed string</returns>
        public static string UpperCaseFirst(this string input)
        {
            return input.Any() ? char.ToUpperInvariant(input[0]) + input.Substring(1) : input;
        }

        /// <summary>
        /// Given the name of a property return an expression representing `t => t.{propertyName}`
        /// </summary>
        /// <typeparam name="T">The type that the property must live on</typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the property doesn't exist on T</exception>
        public static Expression<Func<T, object>> ToPredicate<T>(this string propertyName)
        {
            //Upper case first to account from lower case JSON
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName.UpperCaseFirst());

            if (propertyInfo == null)
            {
                // Someone has provided a property that doesn't exist on the object
                throw new ArgumentException($"Provided property name isn't a member of {typeof(T).Name}", nameof(propertyName));
            }

            var parameterExpression = Expression.Parameter(type);
            var propertyExpression = Expression.PropertyOrField(parameterExpression, propertyInfo.Name);

            return Expression.Lambda<Func<T, object>>(propertyExpression, parameterExpression);
        }
    }
}