using System;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TypeDescriptors;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomArgumentTypeDescriptors : ScenarioTestBase<CustomArgumentTypeDescriptors>
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public CustomArgumentTypeDescriptors(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<App>("Basic Help - includes param")
                {
                    And = {AppSettings = BasicHelp.Clone(a => a.ArgumentTypeDescriptors.Add(new SquareTypeDescriptor()))},
                    WhenArgs = "Do -h",
                    Then = {Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  square

Options:
  -h | --help  Show help information" }
                },
                new Given<App>("Detailed Help - includes param and display name from descriptor")
                {
                    And = {AppSettings = DetailedHelp.Clone(a => a.ArgumentTypeDescriptors.Add(new SquareTypeDescriptor()))},
                    WhenArgs = "Do -h",
                    Then = {Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  square    <!!SQUARE!!>


Options:

  -h | --help
  Show help information" }
                },
                new Given<App>("exec - parses argument using descriptor")
                {
                    And = {AppSettings = BasicHelp.Clone(a => a.ArgumentTypeDescriptors.Add(new SquareTypeDescriptor()))},
                    WhenArgs = "Do 2x3",
                    Then = {Outputs = { new Square{Length = 2, Width = 3} }}
                },
                new Given<App>("exec - fails with clear message when descriptor is not registered")
                {
                    WhenArgs = "Do ",
                    Then =
                    {
                        ExitCode = 2,
                        Result = "type : CommandDotNet.Tests.FeatureTests.CustomArgumentTypeDescriptors+Square is not supported. " +
                                 "If it's an argument model, inherit from IArgumentModel, " +
                                 "otherwise implement a TypeConverter or IArgumentTypeDescriptor to support this type."
                    }
                },
            };

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