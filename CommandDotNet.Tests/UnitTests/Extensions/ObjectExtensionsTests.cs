using System;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Extensions
{
    public class ObjectExtensionsTests
    {
        public class ToStringFromPublicProperties
        {
            [Fact]
            public void Null_object_throws_ArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => 
                        ((object) null!).ToStringFromPublicProperties())
                    .ParamName.Should().Be("item");
            }

            [Fact]
            public void Includes_properties_without_values()
            {
                new TestObject().ToStringFromPublicProperties().Should().Be(
                    @"TestObject:
Error: 
Settings: 
Text: ");
            }

            [Fact]
            public void Indents_properties_with_IIndentable_values()
            {
                new TestObject(settings: new AppHelpSettings())
                    .ToStringFromPublicProperties(new Indent(depth: 2)).Should().Be(
                        @"    TestObject:
    Error: 
    Settings:       AppHelpSettings:
      ExpandArgumentsInUsage: True
      PrintHelpOption: False
      TextStyle: Detailed
      UsageAppName: 
      UsageAppNameStyle: Adaptive
    Text: ");
            }

            [Fact]
            public void Indents_properties_with_Exception_values_and_includes_properties_and_data()
            {
                new TestObject(error:TestException.Instance)
                    .ToStringFromPublicProperties(new Indent(depth: 2)).Should().Be(
                        @"    TestObject:
    Error:       CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
      Properties:
        SomeProperty: Some property value
      Data:
        data-key: data value
        non-serializable-key: non-serializable-value
    Settings: 
    Text: ");
            }
        }

        private class TestObject
        {
            public string? Text { get; }
            // an IIndentable
            public AppHelpSettings? Settings { get; }
            public Exception? Error { get; }

            public TestObject(string? text = null, AppHelpSettings? settings = null, Exception? error = null)
            {
                Text = text;
                Settings = settings;
                Error = error;
            }
        }
    }
}