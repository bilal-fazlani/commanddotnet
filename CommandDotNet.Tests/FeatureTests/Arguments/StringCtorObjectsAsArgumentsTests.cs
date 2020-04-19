using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class StringCtorObjectsAsArgumentsTests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public StringCtorObjectsAsArgumentsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BasicHelp_Includes_StringCtorObjects()
        {
            new AppRunner<App>(BasicHelp).Verify(_output, new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:
  arg
"
                }
            });
        }

        [Fact]
        public void BasicHelp_List_Includes_StringCtorObjects()
        {
            new AppRunner<App>(BasicHelp).Verify(_output, new Scenario
            {
                When = {Args = "DoList -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll DoList [arguments]

Arguments:
  args
"
                }
            });
        }

        [Fact]
        public void DetailedHelp_Includes_StringCtorObjects()
        {
            new AppRunner<App>(DetailedHelp).Verify(_output, new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:

  arg  <FILENAME>
"
                }
            });
        }

        [Fact]
        public void DetailedHelp_List_Includes_StringCtorObjects()
        {
            new AppRunner<App>(DetailedHelp).Verify(_output, new Scenario
            {
                When = {Args = "DoList -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll DoList [arguments]

Arguments:

  args (Multiple)  <FILENAME>
"
                }
            });
        }

        [Fact]
        public void Exec_ConvertsStringToObject()
        {
            new AppRunner<App>().Verify(_output, new Scenario
            {
                When = {Args = "DoList some-value another-value"},
                Then =
                {
                    Captured =
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
            new AppRunner<App>().Verify(_output, new Scenario
            {
                When = {Args = "Do some-value"},
                Then = { Captured = { new StringCtorObject("some-value") } }
            });
        }

        private class App
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do(StringCtorObject arg)
            {
                TestCaptures.Capture(arg);
            }

            public void DoList(List<StringCtorObject> args)
            {
                TestCaptures.Capture(args);
            }
        }

        private class StringCtorObject
        {
            public string Filename { get; }

            public StringCtorObject(string filename)
            {
                Filename = filename;
            }
        }
    }
}
