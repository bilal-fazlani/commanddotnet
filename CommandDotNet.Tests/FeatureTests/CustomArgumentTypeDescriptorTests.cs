using System;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using CommandDotNet.TypeDescriptors;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomArgumentTypeDescriptorTests : TestBase
    {
        private static readonly AppSettings BasicHelpWithDescriptor = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelpWithDescriptor = TestAppSettings.DetailedHelp;

        public CustomArgumentTypeDescriptorTests(ITestOutputHelper output) : base(output)
        {
            var descriptor = new SquareTypeDescriptor();
            BasicHelpWithDescriptor.ArgumentTypeDescriptors.Add(descriptor);
            DetailedHelpWithDescriptor.ArgumentTypeDescriptors.Add(descriptor);
        }

        [Fact]
        public void BasicHelp_IncludesParam()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = BasicHelpWithDescriptor },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:
  square" }
            });
        }

        [Fact]
        public void DetailedHelp_IncludesParamAndDisplayNameFromDecriptor()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = DetailedHelpWithDescriptor },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:

  square  <!!SQUARE!!>" }
            });
        }

        [Fact]
        public void Exec_ParseArgumentUsingDescriptor()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = BasicHelpWithDescriptor },
                WhenArgs = "Do 2x3",
                Then = { Outputs = { new Square { Length = 2, Width = 3 } } }
            });
        }

        [Fact]
        public void Exec_WhenDescriptorIsNotRegistered_FailsWithActionableMessage()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "Do ",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts =
                    {
                        "type : CommandDotNet.Tests.FeatureTests.CustomArgumentTypeDescriptorTests+Square is not supported. ",
                        "If it is an argument model, inherit from IArgumentModel. ",
                        "Otherwise, to support this type, implement a TypeConverter or IArgumentTypeDescriptor or add a constructor with a single string parameter."
                    }
                }
            });
        }

        public class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do(Square square)
            {
                TestOutputs.Capture(square);
            }
        }

        public class Square
        {
            public int Length { get; set; }
            public int Width { get; set; }

            public static Square Parse(string value)
            {
                var parts = value.Split('x');
                var length = int.Parse(parts[0]);
                var width = int.Parse(parts[1]);
                return new Square { Length = length, Width = width };
            }
        }

        public class SquareTypeDescriptor : IArgumentTypeDescriptor
        {
            public bool CanSupport(Type type)
            {
                return type == typeof(Square);
            }

            public string GetDisplayName(IArgument argument)
            {
                return $"!!{nameof(Square)}!!";
            }

            public object ParseString(IArgument argument, string value)
            {
                return Square.Parse(value);
            }
        }
    }
}