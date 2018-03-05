using System;

namespace Audacia.Core.Time
{
    /// <summary>
    /// Stores a timespan as separate units of time
    /// </summary>
    public sealed class TimeContainer
    {
        public double Years { get; private set; }
        public double Months { get; private set; }
        public double Weeks { get; private set; }
        public double Days { get; private set; }
        public double Hours { get; private set; }
        public double Minutes { get; private set; }
        public double Seconds { get; private set; }
        public double Milliseconds { get; private set; }
        
        /// <summary>
        /// Creates a time container from a timespan.
        /// The sum of all the properties should match the timespan.
        /// </summary>
        /// <param name="duration">a length of time</param>
        /// <returns>A relative time container</returns>
        public static TimeContainer CreateRelativeTimeContainer(TimeSpan duration)
        {
            var delta = GetInitialDeltaFromTimeSpan(duration);
            return new TimeContainer
            {
                Years = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Year),
                Months = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Month),
                Weeks = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Week),
                Days = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Day),
                Hours = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Hour),
                Minutes = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Minute),
                Seconds = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Second),
                Milliseconds = GetTotalUnitsAndRemoveDeltaUnits(ref delta, TimeConstants.Millisecond)
            };
        }

        /// <summary>
        /// Creates a time container from a timespan.
        /// Each property is the total possible of that unit
        /// from the whole timespan.
        /// </summary>
        /// <param name="duration">a length of time</param>
        /// <returns>A total time container</returns>
        public static TimeContainer CreateTotalTimeContainer(TimeSpan duration)
        {
            var delta = GetInitialDeltaFromTimeSpan(duration);
            return new TimeContainer
            {
                Years = GetTotalUnits(delta, TimeConstants.Year),
                Months = GetTotalUnits(delta, TimeConstants.Month),
                Weeks = GetTotalUnits(delta, TimeConstants.Week),
                Days = GetTotalUnits(delta, TimeConstants.Day),
                Hours = GetTotalUnits(delta, TimeConstants.Hour),
                Minutes = GetTotalUnits(delta, TimeConstants.Minute),
                Seconds = GetTotalUnits(delta, TimeConstants.Second),
                Milliseconds = GetTotalUnits(delta, TimeConstants.Millisecond)
            };
        }

        private static double GetInitialDeltaFromTimeSpan(TimeSpan duration)
        {
            return Math.Abs(duration.TotalMilliseconds);
        }

        /// <summary>
        /// Gets the size of the delta from the total units and the unit size
        /// </summary>
        /// <param name="totalUnits">Total units in delta</param>
        /// <param name="unitSize">Size of a single unit</param>
        /// <returns>Size in delta units</returns>
        private static double GetDeltaUnits(double totalUnits, double unitSize)
        {
            return totalUnits * unitSize;
        }

        /// <summary>
        /// Gets the total number of units in the delta
        /// </summary>
        /// <param name="delta">Duration in milliseconds</param>
        /// <param name="unitSize">Unit size</param>
        /// <returns>Total units in delta</returns>
        private static double GetTotalUnits(double delta, double unitSize)
        {
            return Math.Floor(delta / unitSize);
        }
        
        /// <summary>
        /// Gets the total number of units in the current delta
        /// Then removes the delta size of the total units from the current delta
        /// </summary>
        /// <param name="delta">Current delta</param>
        /// <param name="unitSize">Size of a single unit</param>
        /// <returns>Total units in delta</returns>
        private static double GetTotalUnitsAndRemoveDeltaUnits(ref double delta, double unitSize)
        {
            var units = GetTotalUnits(delta, unitSize);
            delta -= GetDeltaUnits(units, unitSize);
            return units;
        }
    }
}
