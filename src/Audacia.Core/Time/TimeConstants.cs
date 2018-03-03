namespace Audacia.Core.Time
{
    /// <summary>
    /// Provides constant values of time in milliseconds
    /// </summary>
    public sealed class TimeConstants
    {
        public const double Millisecond = 1;
        public const double Second = Millisecond * 1000;
        public const double Minute = Second * 60;
        public const double Hour = Minute * 60;
        public const double Day = Hour * 24;
        public const double Week = Day * 7;
        public const double Month = Day * 30;
        public const double Year = Day * 365;
    }
}
