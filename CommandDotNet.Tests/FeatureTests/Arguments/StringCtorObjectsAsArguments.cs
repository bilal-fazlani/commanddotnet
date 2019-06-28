using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class StringCtorObjectsAsArguments : TestBase
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public StringCtorObjectsAsArguments(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void BasicHelp_Includes_StringCtorObjects()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = BasicHelp},
                WhenArgs = "Do -h",
                Then = {Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  arg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void BasicHelp_List_Includes_StringCtorObjects()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "DoList -h",
                Then = { Result = @"Usage: dotnet testhost.dll DoList [arguments] [options]

Arguments:
  args

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void DetailedHelp_Includes_StringCtorObjects()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = DetailedHelp},
                WhenArgs = "Do -h",
                Then = {Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  arg    <FILENAME>


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void DetailedHelp_List_Includes_StringCtorObjects()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "DoList -h",
                Then = { Result = @"Usage: dotnet testhost.dll DoList [arguments] [options]

Arguments:

  args (Multiple)    <FILENAME>


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void Exec_ConvertsStringToObject()
        {
            Verify(new Given<App>
            {
                WhenArgs = "DoList some-value another-value",
                Then =
                {
                    Outputs =
                    {
                        new List<StringCtorObject>
                        {
                            new StringCtorObject("some-value"),
                            new StringCtorObject("another-value")
                        }
                    }
                }
            });
        }

        [Fact]
        public void Exec_List_ConvertsStringToObject()
        {
            Verify(new Given<App>
            {
                WhenArgs = "Do some-value",
                Then = { Outputs = { new StringCtorObject("some-value") } }
            });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do(StringCtorObject arg)
            {
                TestOutputs.Capture(arg);
            }

            public void DoList(List<StringCtorObject> args)
            {
                TestOutputs.Capture(args);
            }
        }

        public class StringCtorObject
        {
            public string Filename { get; }

            public StringCtorObject(string filename)
            {
                Filename = filename;
            }
        }
    }
}