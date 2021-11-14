using CommandDotNet.TypeDescriptors;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.TypeDescriptors
{
    public class StringCtorTypeDescriptorTest
    {
        [Fact]
        public void CanSupport_ctor_with_single_string()
        {
            new StringCtorTypeDescriptor().CanSupport(typeof(StringCtor)).Should().BeTrue();
        }

        [Fact]
        public void CanSupport_static_parse_method_with_single_string()
        {
            new StringCtorTypeDescriptor().CanSupport(typeof(StaticParse)).Should().BeTrue();
        }

        [Fact]
        public void CanSupport_static_parse_method_with_single_required_param_of_type_string()
        {
            new StringCtorTypeDescriptor().CanSupport(typeof(StaticParseWithOptional)).Should().BeTrue();
        }

        public class StringCtor
        {
            public string Value { get; }

            public StringCtor(string value)
            {
                Value = value;
            }
        }

        public class StaticParse
        {
            public string Value { get; private set; } = null!;

            public static StaticParse Parse(string value) => new() { Value = value };
        }

        public class StaticParseWithOptional
        {
            public string Value { get; private set; } = null!;

            public static StaticParseWithOptional Parse(string value, bool optional = true) => new() { Value = value };
        }
    }
}