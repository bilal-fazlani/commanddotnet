using System;
using System.Collections;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests
{
    public class ArgumentArityTests
    {
        private const bool NoDefault = false;
        private const bool HasDefault = true;
        
        [Theory]
        [InlineData(typeof(string), NoDefault, 1, 1)]
        [InlineData(typeof(string), HasDefault, 0, 1)]
        [InlineData(typeof(int), NoDefault, 1, 1)]
        [InlineData(typeof(int), HasDefault, 0, 1)]
        [InlineData(typeof(int?), NoDefault, 0, 1)]
        [InlineData(typeof(int?), HasDefault, 0, 1)]
        [InlineData(typeof(object), NoDefault, 1, 1)]
        [InlineData(typeof(object), HasDefault, 0, 1)]
        [InlineData(typeof(IEnumerable), NoDefault, 1, int.MaxValue)]
        [InlineData(typeof(IEnumerable), HasDefault, 0, int.MaxValue)]
        public void Default(Type type, bool hasDefaultValue, int expectedMin, int expectedMax)
        {
            var actual = ArgumentArity.Default(type, hasDefaultValue, BooleanMode.Explicit);
            var expected = new ArgumentArity(expectedMin, expectedMax);
            expected.Should().Be(actual);
        }

        [Theory]
        [InlineData(BooleanMode.Explicit, NoDefault, 1, 1)]
        [InlineData(BooleanMode.Explicit, HasDefault, 0, 1)]
        [InlineData(BooleanMode.Implicit, NoDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, HasDefault, 0, 0)]
        public void DefaultBool(BooleanMode booleanMode, bool hasDefaultValue, int expectedMin, int expectedMax)
        {
            var actual = ArgumentArity.Default(typeof(bool), hasDefaultValue, booleanMode);
            var expected = new ArgumentArity(expectedMin, expectedMax);
            expected.Should().Be(actual);
        }

        [Theory]
        [InlineData(BooleanMode.Explicit, NoDefault, 0 , 1)]
        [InlineData(BooleanMode.Explicit, HasDefault, 0, 1)]
        [InlineData(BooleanMode.Implicit, NoDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, HasDefault, 0, 0)]
        public void DefaultNullableBool(BooleanMode booleanMode, bool hasDefaultValue, int expectedMin, int expectedMax)
        {
            var actual = ArgumentArity.Default(typeof(bool?), hasDefaultValue, booleanMode);
            var expected = new ArgumentArity(expectedMin, expectedMax);
            expected.Should().Be(actual);
        }

        [Fact]
        public void DefaultBooleanModeCannotBeUnknown()
        {
            Assert.Throws<ArgumentException>(
                    () => ArgumentArity.Default(typeof(bool), NoDefault, BooleanMode.Unknown))
                .Message.Should().Be("booleanMode cannot be Unknown");
        }
    }
}