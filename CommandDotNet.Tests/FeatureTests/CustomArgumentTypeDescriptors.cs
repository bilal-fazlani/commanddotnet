using System;
using CommandDotNet.ClassModeling;
using CommandDotNet.Help;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TypeDescriptors;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomArgumentTypeDescriptors : TestBase
    {
        private static readonly AppSettings BasicHelpWithDescriptor = new AppSettings { Help = { TextStyle = HelpTextStyle.Basic }, EnableVersionOption = false };
        private static readonly AppSettings DetailedHelpWithDescriptor = new AppSettings { Help = { TextStyle = HelpTextStyle.Detailed }, EnableVersionOption = false };

        public CustomArgumentTypeDescriptors(ITestOutputHelper output) : base(output)
        {
            var descriptor = new SquareTypeDescriptor();
            BasicHelpWithDescriptor.ArgumentTypeDescriptors.Add(descriptor);
            DetailedHelpWithDescriptor.ArgumentTypeDescriptors.Add(descriptor);
        }

        [Fact]
        public void BasicHelp_IncludesParam()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelpWithDescriptor },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  square

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void DetailedHelp_IncludesParamAndDisplayNameFromDecriptor()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelpWithDescriptor },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  square    <!!SQUARE!!>


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void Exec_ParseArgumentUsingDescriptor()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelpWithDescriptor },
                WhenArgs = "Do 2x3",
                Then = { Outputs = { new Square { Length = 2, Width = 3 } } }
            });
        }

        [Fact]
        public void Exec_FailsWithClearMessageWhenDescriptorIsNotRegistered()
        {
            Verify(new Given<App>
            {
                WhenArgs = "Do ",
                Then =
                {
                    ExitCode = 2,
                    Result = "type : CommandDotNet.Tests.FeatureTests.CustomArgumentTypeDescriptors+Square is not supported. " +
                             "If it's an argument model, inherit from IArgumentModel, " +
                             "otherwise implement a TypeConverter or IArgumentTypeDescriptor to support this type."
                }
            });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

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
                var parts = value.Split("x");
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

            public string GetDisplayName(ArgumentInfo argumentInfo)
            {
                return $"!!{nameof(Square)}!!";
            }

            public object ParseString(ArgumentInfo argumentInfo, string value)
            {
                return Square.Parse(value);
            }
        }
    }
}