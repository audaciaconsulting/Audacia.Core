using System;

namespace Audacia.Core
{
    /// <summary>
    /// An interface to provide a DateTime properties to other functionality, to allow for better mocking and testing
    /// </summary>
    public interface IDateTimeOffsetProvider
    {
        /// <summary>
        /// Mockable DateTimeOffset.UtcNow replacement
        /// </summary>
        DateTimeOffset UtcNow { get; }
        /// <summary>
        /// Mockable DateTimeOffset.Now replacement
        /// </summary>
        DateTimeOffset Now { get; }
        /// <summary>
        /// Mockable DateTime.Today replacement
        /// </summary>
        DateTimeOffset Today { get; }
        /// <summary>
        /// Mockable DateTime.Today replacement, but being the date from UtcNow
        /// </summary>
        DateTimeOffset UtcToday { get; }
    }
}
