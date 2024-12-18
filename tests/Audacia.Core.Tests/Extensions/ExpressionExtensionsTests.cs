using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Audacia.Core.Extensions;
using FluentAssertions;
using Xunit;

namespace Audacia.Core.Tests.Extensions
{
    public class ExpressionExtensionsTests
    {
        [Fact]
        public void Generic_expression_is_converted()
        {
            Expression<Func<IHavePet<IPet>, bool>> hasPet =
                r => r.Pets.Any();
            var dogOwner = new DogOwner()
            {
                Pets = new List<Dog>
                {
                    new() { Name = "Toby" }
                }
            };

            var convertedExpression = hasPet
                .ConvertGenericTypeArgument<IHavePet<IPet>, DogOwner, bool>()!;

            var result = convertedExpression.Compile().Invoke(dogOwner);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Toby", true)]
        [InlineData("Deedee", false)]
        public void Generic_expression_with_generic_lambda_is_converted(string dogName, bool expectedValue)
        {
            Expression<Func<IHavePet<IPet>, bool>> hasPetCalledToby =
                r => r.Pets.Any(p => p.Name == "Toby");
            var dogOwner = new DogOwner()
            {
                Pets = new List<Dog>
                {
                    new() { Name = dogName }
                }
            };

            var convertedExpression = hasPetCalledToby
                .ConvertGenericTypeArgument<IHavePet<IPet>, DogOwner, bool>()!;

            var result = convertedExpression.Compile().Invoke(dogOwner);

            result.Should().Be(expectedValue);
        }

        private interface IHavePet<TPet> where TPet : IPet
        {
            ICollection<TPet> Pets { get; set; }
        }

        private interface IPet
        {
            public string Name { get; set; }
        }

        private class Dog : IPet
        {
            public string Name { get; set; }
        }

        private class DogOwner : IHavePet<Dog>
        {
            public ICollection<Dog> Pets { get; set; } = new List<Dog>();
        }
    }
}