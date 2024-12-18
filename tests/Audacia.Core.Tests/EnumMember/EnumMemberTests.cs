using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;
using Xunit;

namespace Audacia.Core.Tests.EnumMember
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names - Makes it easier to read.")]
    public static class EnumMemberTests
    {
        private enum Day
        {
            [EnumMember(Value = "Church Day")]
            Sunday,

            Monday,

            Tuesday,

            [Display(Name = "Mid-week")]
            Wednesday,

            Thursday,

            [Description("A fun day of the week")]
            Friday,

            Saturday
        }

        [Fact]
        public static void Check_if_enum_member_parsing_returns_expected_result()
        {
            //Arrange 
            const string value = "Church Day";
            const Day expectedResult = Day.Sunday;

            //Act
            var result = Core.EnumMember.Parse<Day>(value);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void Check_if_description_parsing_returns_expected_result()
        {
            //Arrange 
            const string value = "A fun day of the week";
            const Day expectedResult = Day.Friday;

            //Act
            var result = Core.EnumMember.Parse<Day>(value);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void Check_if_display_parsing_returns_expected_result()
        {
            //Arrange 
            const string value = "Mid-week";
            const Day expectedResult = Day.Wednesday;

            //Act
            var result = Core.EnumMember.Parse<Day>(value);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void Check_that_parsing_with_no_attribute_returns_expected_result()
        {
            //Arrange 
            const string value = "Saturday";
            const Day expectedResult = Day.Saturday;

            //Act
            var result = Core.EnumMember.Parse<Day>(value);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public static void Check_that_overflow_exception_is_correctly_called()
        {
            //Arrange 
            const string value = "100";

            //Act
            var exception = Assert.Throws<OverflowException>(() => Core.EnumMember.Parse<Day>(value));

            //Assert
            Assert.Equal("Value '100' is outside the range of 'Day'.", exception.Message);
        }

        [Fact]
        public static void Check_that_argument_exception_is_correctly_thrown()
        {
            //Arrange 
            const string value = "March";

            //Act
            var exception = Assert.Throws<ArgumentException>(() => Core.EnumMember.Parse<Day>(value));

            //Assert
            Assert.Equal("Requested value 'March' was not found.", exception.Message);
        }

        [Fact]
        public static void Check_that_string_enum_value_gives_correct_member()
        {
            //Arrange 
            var expected = new Day[]
            {
                Day.Wednesday,
                Day.Tuesday,
                Day.Thursday
            };

            var stringMembers = new string[]
            {
                "3",
                "2",
                "4"
            };

            var actual = new List<Day>();
            var dayType = typeof(Day);
            //Act
            foreach (var day in stringMembers)
            {
                var result = default(Day);
                if (Core.EnumMember.TryParse(dayType, day, out var enumValue))
                {
                    result = (Day)enumValue!;
                    Debug.Assert(enumValue != null, nameof(enumValue) + " != null");
                }

                actual.Add(result);
            }

            //Assert
            Assert.Equal(expected, actual.ToArray());
        }
    }
}