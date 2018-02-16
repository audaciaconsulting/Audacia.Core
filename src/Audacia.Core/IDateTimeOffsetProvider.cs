using System;

namespace Audacia.Core
{
    /// <summary>
    /// An interface to provide a DateTime properties to other functionality, to allow for better mocking and testing
    /// </summary>
    public interface IDateTimeOffsetProvider
    {
        /// <summary>
        /// Mockable DateTimeOffset.Now replacement
        /// </summary>
        DateTimeOffset Now { get; }
        /// <summary>
        /// Mockable DateTime.Today replacement
        /// </summary>
        DateTime Today { get; }
    }
}