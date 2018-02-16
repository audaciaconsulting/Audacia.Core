using System;

namespace Audacia.Core
{
    /// <summary>
    /// A default instance of DateTime provider to provide the standard Now, UtcNow and Today functionality
    /// </summary>
    public static class DateTimeOffsetProvider
    {
        private static readonly IDateTimeOffsetProvider DefaultProviderInstance = new DefaultDateTimeProvider();

        /// <summary>
        /// Default Instance as a singleton
        /// </summary>
        public static IDateTimeOffsetProvider Instance { get; } = DefaultProviderInstance;

        private class DefaultDateTimeProvider : IDateTimeOffsetProvider
        {
            public DateTimeOffset Now => DateTimeOffset.Now;
            public DateTime Today => DateTime.Today;
        }
    }
}