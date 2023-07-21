using System.IO;

namespace Audacia.Core.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Copies a stream in to a new memory stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="close">Whether to close the original stream.</param>
        /// <returns>The copied stream.</returns>
        public static MemoryStream ToMemoryStream(this Stream source, bool close = false)
        {
            const int readSize = 256;
            var buffer = new byte[readSize];
            var memoryStream = new MemoryStream();

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
}