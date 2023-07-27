using System;
using System.IO;

namespace Audacia.Core.Extensions;

/// <summary>
/// Extension methods for the type <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Copies a stream in to a new memory stream.
    /// </summary>
    /// <param name="source">The source stream.</param>
    /// <param name="close">Whether to close the original stream.</param>
    /// <returns>The copied stream.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?", Justification = "Easy to understand and implement.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "ACL1002:Member or local function contains too many statements", Justification = "Easier to read and understand.")]
    public static MemoryStream ToMemoryStream(this Stream source, bool close = false)
    {
        const int readSize = 256;
        var buffer = new byte[readSize];
        var memoryStream = new MemoryStream();

        ArgumentNullException.ThrowIfNull(source);

        var count = source.Read(buffer, 0, readSize);

        while (count > 0)
        {
            memoryStream.Write(buffer, 0, count);
            count = source.Read(buffer, 0, readSize);
        }

        memoryStream.Position = 0;

        if (close)
        {
            source.Close();
        }

        return memoryStream;
    }
}