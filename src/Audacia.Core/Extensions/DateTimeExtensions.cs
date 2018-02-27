using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Audacia.Core.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a suffix depending on the input date (i.e. nd, rd, th)
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns>The suffix string</returns>
        public static string TwoLetterSuffix(this DateTime date)
        {
            var dayMod10 = date.Day % 10;

            if (dayMod10 > 3 || date.Day >= 10 && date.Day <= 19 || dayMod10 == 0)
            {
                return "th";
            }

            switch (dayMod10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                default:
                    return "rd";
            }
        }

        /// <summary>
        /// If a month is longer than the requested month, will choose the last day of
        /// the requested month instead of continuing into the start of the month after
        /// </summary>
        /// <param name="date">The start date</param>
        /// <param name="months">The number of months to add</param>
        /// <returns>The resultant date</returns>
        public static DateTime AddJustMonths(this DateTime date, int months)
        {
            var firstDayOfTargetMonth = new DateTime(date.Year, date.Month, 1).AddMonths(months);
            var lastDayofTargetMonth = DateTime.DaysInMonth(firstDayOfTargetMonth.Year, firstDayOfTargetMonth.Month);

            var targetDay = date.Day > lastDayofTargetMonth ? lastDayofTargetMonth : date.Day;

            return new DateTime(firstDayOfTargetMonth.Year, firstDayOfTargetMonth.Month, targetDay);
        }

        /// <summary>
        /// Add business days to a date (does not support bank holidays)
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="businessDays">The number of days to add</param>
        /// <param name="skipDays">Days to skip (e.g. bank holidays)</param>
        /// <returns>The resultant date</returns>
        public static DateTime AddBusinessDays(this DateTime startDate, double businessDays, IEnumerable<DateTime> skipDays = null)
        {
            if (skipDays == null)
            {
                skipDays = new DateTime[0];
            }

            var dayOfWeek = (int)startDate.DayOfWeek;
            var temp = businessDays + dayOfWeek + 1;

            if (dayOfWeek != 0)
            {
                temp--;
            }

            var startDateWithWorkingDaysAdded = startDate.AddDays(Math.Floor(temp / 5) * 2 - dayOfWeek + temp - 2 * Convert.ToInt32(Math.Abs(temp % 5 - 0) < double.Epsilon));

            return startDateWithWorkingDaysAdded.AddDays(skipDays.Count(bh => bh >= startDate && bh <= startDateWithWorkingDaysAdded));
        }

        /// <summary>
        /// Determines whether or not the current date is a business day
        /// </summary>
        /// <param name="date">The date to test</param>
        /// <param name="skipDays">Days to skip (e.g. bank holidays)</param>
        /// <returns></returns>
        public static bool IsBusinessDay(this DateTime date, IEnumerable<DateTime> skipDays = null)
        {
            if (skipDays == null)
            {
                skipDays = new DateTime[0];
            }

            var weekendDays = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

            return !weekendDays.Contains(date.DayOfWeek) && !skipDays.Contains(date);
        }

        /// <summary>
        /// Gets the business days between two dates (does not support bank holidays)
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="exclusive">Calculate inclusive or exclusive of start and end date</param>
        /// <param name="skipDays">Days to skip (e.g. bank holidays)</param>
        /// <returns>The number of business days between</returns>
        public static double GetBusinessDaysUntil(this DateTime startDate, DateTime endDate, bool exclusive = false, ICollection<DateTime> skipDays = null)
        {
            if (skipDays == null)
            {
                skipDays = new DateTime[0];
            }

            var firstDay = startDate.Date;
            var lastDay = endDate.Date;
            var isNegative = false;

            if (firstDay > lastDay)
            {
                var temp = firstDay;
                firstDay = lastDay;
                lastDay = temp;
                isNegative = true;
            }

            var businessDays = 0;

            for (var date = firstDay; exclusive ? date < lastDay : date <= lastDay; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday &&
                    !skipDays.Contains(date))
                {
                    businessDays++;
                }
            }

            return isNegative ? 0 - businessDays : businessDays;
        }
    }
}