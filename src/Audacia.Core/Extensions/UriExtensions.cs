using System;
using System.Linq;

namespace Audacia.Core.Extensions;

/// <summary>
/// Extension methods for the type <see cref="Uri"/>.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Append additional sub-paths to a URI.
    /// </summary>
    /// <param name="uri">The uri URI.</param>
    /// <param name="paths">Paths to append.</param>
    /// <returns>The appended URI.</returns>
    public static Uri Append(this Uri uri, params string[] paths)
    {
        uri = new Uri(paths.Aggregate(
            uri?.AbsoluteUri ?? string.Empty,
            (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));

        return uri;
    }

    /// <summary>
    /// Gets the domain from a uri.
    /// </summary>
    /// <param name="input">The uri URI.</param>
    /// <returns>The domain.</returns>
    public static string GetDomain(this Uri input)
    {
        return input == null
            ? string.Empty
            : input.Scheme + Uri.SchemeDelimiter + input.Host +
               (input.IsDefaultPort ? string.Empty : ":" + input.Port);
    }
}