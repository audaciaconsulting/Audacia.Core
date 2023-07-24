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
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <returns>The copied stream.</returns>
    public static MemoryStream ToMemoryStream(this Stream source)
    {
        const int readSize = 256;
        var buffer = new byte[readSize];
        var memoryStream = new MemoryStream();

        var count = source?.Read(buffer, 0, readSize) ?? throw new ArgumentNullException(nameof(source), "Source can not be null");

        while (count > 0)
        {
            memoryStream.Write(buffer, 0, count);
            count = source.Read(buffer, 0, readSize);
        }

        memoryStream.Position = 0;

        return memoryStream;
    }

    /// <summary>
    /// Copies a stream in to a new memory stream and closes it once complete.
    /// </summary>
    /// <param name="source">The source stream.</param>
    /// <returns>The copied stream.</returns>
    public static MemoryStream ToMemoryStreamThenClose(this Stream source)
    {
        var memoryStream = ToMemoryStream(source);

        source.Close();

        return memoryStream;
    }
}