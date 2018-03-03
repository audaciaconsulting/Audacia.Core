using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Audacia.Core.Extensions;

namespace Audacia.Core.Time
{
    /// <summary>
    /// Helps you to build relative time strings
    /// </summary>
    public sealed class TimeBuilder
    {
        private readonly ICollection<string> _timeCollection = new HashSet<string>();

        public TimeContainer TotalTime { get; private set; }

        public TimeContainer RelativeTime { get; private set; }

        public TimeBuilder()
        {
            TotalTime = new TimeContainer();
            RelativeTime = new TimeContainer();
        }

        public TimeBuilder(TimeSpan duration)
        {
            TotalTime = TimeContainer.CreateTotalTimeContainer(duration);
            RelativeTime = TimeContainer.CreateRelativeTimeContainer(duration);
        }
        
        /// <summary>
        /// Adds property from total time if predicate returns true,
        /// Otherwise adds property from relative time,
        /// Ignores properties of zero value
        /// </summary>
        /// <param name="property">Value of time to add</param>
        /// <param name="useTotalOrRelative">predicate</param>
        /// <returns>The current instance of TimeBuilder</returns>
        public TimeBuilder AddIf(Expression<Func<TimeContainer, double>> property, Expression<Func<TimeBuilder, bool>> useTotalOrRelative)
        {
            const bool ignoreZero = true;
            return useTotalOrRelative.Compile().Invoke(this)
                ? AddTotal(property, ignoreZero)
                : AddRelative(property, ignoreZero);
        }

        /// <summary>
        /// Adds property from relative time
        /// </summary>
        /// <param name="property">Value of time to add</param>
        /// <param name="ignoreZero">will ignore the value if it is zero</param>
        /// <returns>The current instance of TimeBuilder</returns>
        public TimeBuilder AddRelative(Expression<Func<TimeContainer, double>> property, bool ignoreZero = false)
        {
            var unitName = ExpressionExtensions.GetPropertyInfo(property).Name;
            var value = property.Compile().Invoke(RelativeTime);
            if (!ignoreZero || value > 0)
            {
                _timeCollection.Add(FormatString(value, unitName));
            }
            return this;
        }
        
        /// <summary>
        /// Adds property from total time
        /// </summary>
        /// <param name="property">Value of time to add</param>
        /// <param name="ignoreZero">will ignore the value if it is zero</param>
        /// <returns>The current instance of TimeBuilder</returns>
        public TimeBuilder AddTotal(Expression<Func<TimeContainer, double>> property, bool ignoreZero = false)
        {
            var unitName = ExpressionExtensions.GetPropertyInfo(property).Name;
            var value = property.Compile().Invoke(TotalTime);
            if (!ignoreZero || value > 0)
            {
                _timeCollection.Add(FormatString(value, unitName));
            }
            return this;
        }

        /// <summary>
        /// Builds a string from the added properties
        /// </summary>
        /// <returns>A built time string</returns>
        public string Build()
        {
            const string prefixComma = ", ";
            var allButOne = _timeCollection.Count - 1;

            var stringBuilder = new StringBuilder();

            if (allButOne > 0)
            {
                stringBuilder.Append(string.Join(prefixComma, _timeCollection.Take(allButOne)));
                stringBuilder.Append(" and ");
                stringBuilder.Append(_timeCollection.Skip(allButOne).Single());
            }
            else
            {
                stringBuilder.Append(string.Join(prefixComma, _timeCollection));
            }
            
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Clears all the properties that have currently been added
        /// </summary>
        /// <returns>The current instance of TimeBuilder</returns>
        public TimeBuilder Clear()
        {
            _timeCollection.Clear();
            return this;
        }

        /// <summary>
        /// Populates the total and relative time properties from the given duration
        /// and clears all the properties that have currently been added
        /// </summary>
        /// <param name="duration">A length of time</param>
        /// <returns>The current instance of TimeBuilder</returns>
        public TimeBuilder FromDuration(TimeSpan duration)
        {
            TotalTime = TimeContainer.CreateTotalTimeContainer(duration);
            RelativeTime = TimeContainer.CreateRelativeTimeContainer(duration);
            Clear();
            return this;
        }

        /// <summary>
        /// Creates a relative time string from a timespan
        /// </summary>
        /// <param name="duration">A length of time</param>
        /// <returns>A relative time string</returns>
        public static string GetDurationAsString(TimeSpan duration)
        {
            return new TimeBuilder(duration)
                .AddIf(t => t.Years, b => true)
                .AddIf(t => t.Months, b => b.TotalTime.Years < 1)
                .AddIf(t => t.Weeks, b => b.TotalTime.Months < 1)
                .AddIf(t => t.Days, b => b.TotalTime.Weeks < 1)
                .AddIf(t => t.Hours, b => b.TotalTime.Days < 1)
                .AddIf(t => t.Minutes, b => b.TotalTime.Hours < 1)
                .AddIf(t => t.Seconds, b => b.TotalTime.Minutes < 1)
                .AddIf(t => t.Milliseconds, b => b.TotalTime.Seconds < 1)
                .Build();
        }

        private static string FormatString(double value, string unitName)
        {
            var isPlural = Math.Floor(value) > 1;
            return $"{value} {(isPlural ? unitName : unitName.TrimEnd('s'))}";
        }
    }
}