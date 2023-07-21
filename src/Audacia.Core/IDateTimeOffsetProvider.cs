using System;

namespace Audacia.Core
{
    /// <summary>
    /// An interface to provide a DateTime properties to other functionality, to allow for better mocking and testing.
    /// </summary>
    public interface IDateTimeOffsetProvider
    {
        /// <summary>
        /// Gets mockable DateTimeOffset.UtcNow replacement.
        /// </summary>
        DateTimeOffset UtcNow { get; }
        /// <summary>
        /// Gets mockable DateTimeOffset.Now replacement.
        /// </summary>
        DateTimeOffset Now { get; }
        /// <summary>
        /// Gets mockable DateTime.Today replacement.
        /// </summary>
        DateTimeOffset Today { get; }
        /// <summary>
        /// Gets mockable DateTime.Today replacement, but being the date from UtcNow.
        /// </summary>
        DateTimeOffset UtcToday { get; }
    }
}
