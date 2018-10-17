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
            public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
            public DateTimeOffset Now => DateTimeOffset.Now;
            public DateTimeOffset Today => DateTime.Today;
            public DateTimeOffset UtcToday => new DateTimeOffset(DateTimeOffset.UtcNow.Date, new TimeSpan());
        }
    }
}
