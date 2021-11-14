using System;
using System.Collections;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests
{
    public class ArgumentArityTests
    {
        private const bool IsOptional = true;
        private const bool HasDefault = true;
        
        [Theory]
        [InlineData(typeof(string), IsOptional, !HasDefault, 0, 1)]
        [InlineData(typeof(string), IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(string), !IsOptional, !HasDefault, 1, 1)]
        [InlineData(typeof(string), !IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(int), IsOptional, !HasDefault, 0, 1)]
        [InlineData(typeof(int), IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(int), !IsOptional, !HasDefault, 1, 1)]
        [InlineData(typeof(int), !IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(int?), IsOptional, !HasDefault, 0, 1)]
        [InlineData(typeof(int?), IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(int?), !IsOptional, !HasDefault, 1, 1)]
        [InlineData(typeof(int?), !IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(object), IsOptional, !HasDefault, 0, 1)]
        [InlineData(typeof(object), IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(object), !IsOptional, !HasDefault, 1, 1)]
        [InlineData(typeof(object), !IsOptional, HasDefault, 0, 1)]
        [InlineData(typeof(IEnumerable), IsOptional, !HasDefault, 0, int.MaxValue)]
        [InlineData(typeof(IEnumerable), IsOptional, HasDefault, 0, int.MaxValue)]
        [InlineData(typeof(IEnumerable), !IsOptional, !HasDefault, 1, int.MaxValue)]
        [InlineData(typeof(IEnumerable), !IsOptional, HasDefault, 0, int.MaxValue)]
        public void Default(Type type, bool isOptional, bool hasDefault, int expectedMin, int expectedMax)
        {
            var actual = ArgumentArity.Default(type, isOptional, hasDefault, BooleanMode.Explicit);
            var expected = new ArgumentArity(expectedMin, expectedMax);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(BooleanMode.Explicit, IsOptional, !HasDefault, 0, 1)]
        [InlineData(BooleanMode.Explicit, IsOptional, HasDefault, 0, 1)]
        [InlineData(BooleanMode.Explicit, !IsOptional, !HasDefault, 1, 1)]
        [InlineData(BooleanMode.Explicit, !IsOptional, HasDefault, 0, 1)]
        [InlineData(BooleanMode.Implicit, IsOptional, !HasDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, IsOptional, HasDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, !IsOptional, !HasDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, !IsOptional, HasDefault, 0, 0)]
        public void DefaultBool(BooleanMode booleanMode, bool isOptional, bool hasDefault, int expectedMin, int expectedMax)
        {
            var actual = ArgumentArity.Default(typeof(bool), isOptional, hasDefault, booleanMode);
            var expected = new ArgumentArity(expectedMin, expectedMax);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(BooleanMode.Explicit, IsOptional, !HasDefault, 0, 1)]
        [InlineData(BooleanMode.Explicit, IsOptional, HasDefault, 0, 1)]
        [InlineData(BooleanMode.Explicit, !IsOptional, !HasDefault, 1, 1)]
        [InlineData(BooleanMode.Explicit, !IsOptional, HasDefault, 0, 1)]
        [InlineData(BooleanMode.Implicit, IsOptional, !HasDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, IsOptional, HasDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, !IsOptional, !HasDefault, 0, 0)]
        [InlineData(BooleanMode.Implicit, !IsOptional, HasDefault, 0, 0)]
        public void DefaultNullableBool(BooleanMode booleanMode, bool isOptional, bool hasDefault, int expectedMin, int expectedMax)
        {
            var actual = ArgumentArity.Default(typeof(bool?), isOptional, hasDefault, booleanMode);
            var expected = new ArgumentArity(expectedMin, expectedMax);
            actual.Should().Be(expected);
        }
    }
}