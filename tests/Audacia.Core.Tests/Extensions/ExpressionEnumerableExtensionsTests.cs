using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Audacia.Core.Extensions;
using FluentAssertions;
using Xunit;

namespace Audacia.Core.Tests.Extensions
{
    public class ExpressionEnumerableExtensionsTests
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(-1, false)]
        [InlineData(12974, false)]
        public void All_returns_false_if_any_expressions_are_false(int idValue, bool expectedResult)
        {
            var expressions = new List<Expression<Func<ExamplePoco, bool>>>
            {
                e => e.Id == 1,
                e => e.Id > 0,
                e => e.Id != 12974
            };
            var target = new ExamplePoco { Id = idValue };

            var sut = expressions.All();
            var result = sut.Compile().Invoke(target);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(-1, false)]
        [InlineData(63245, true)]
        public void Any_returns_false_if_all_expressions_are_false(int idValue, bool expectedResult)
        {
            var expressions = new List<Expression<Func<ExamplePoco, bool>>>
            {
                e => e.Id == 0,
                e => e.Id == 1,
                e => e.Id > 1
            };
            var target = new ExamplePoco { Id = idValue };

            var sut = expressions.Any();
            var result = sut.Compile().Invoke(target);

            result.Should().Be(expectedResult);
        }

        private class ExamplePoco
        {
            public int Id { get; set; }
        }
    }
}