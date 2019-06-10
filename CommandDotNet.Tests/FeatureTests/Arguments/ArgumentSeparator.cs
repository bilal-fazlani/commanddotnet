using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class ArgumentSeparator : ScenarioTestBase<ArgumentSeparator>
    {
        private static readonly AppSettings SeparatorDisabled = TestAppSettings.BasicHelp;
        private static readonly AppSettings SeparatorEnabledBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.AllowArgumentSeparator = true);
        private static readonly AppSettings SeparatorEnabledDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.AllowArgumentSeparator = true);

        public ArgumentSeparator(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<App>("basic help - enabled - example includes argument separator")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "list -h",
                    Then =
                    {
                        ResultsContainsTexts = { @"Usage: dotnet testhost.dll List [arguments] [options] [[--] <arg>...]" },
                    }
                },
                new Given<App>("detailed help - enabled - example includes argument separator")
                {
                    And = {AppSettings = SeparatorEnabledDetailedHelp},
                    WhenArgs = "list -h",
                    Then =
                    {
                        ResultsContainsTexts = { @"Usage: dotnet testhost.dll List [arguments] [options] [[--] <arg>...]" }
                    }
                },
                new Given<App>("basic help - enabled - no list operand - example does not include argument separator")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "list -h",
                    Then =
                    {
                        ResultsNotContainsTexts = { @"[[--] <arg>...]" }
                    }
                },
                new Given<App>("detailed help - enabled - no list operand - example does not include argument separator")
                {
                    And = {AppSettings = SeparatorEnabledDetailedHelp},
                    WhenArgs = "list -h",
                    Then =
                    {
                        ResultsNotContainsTexts = { @"[[--] <arg>...]" }
                    }
                },
                // TODO: these scenarios need better names after this is working
                new Given<App>("exec - disabled - only list operand  - returns 1 with error output")
                {
                    And = {AppSettings = SeparatorDisabled},
                    WhenArgs = "list -- a1 b2 c3",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized option '--'"}
                    }
                },
                // TODO: these scenarios need better names after this is working
                new Given<App>("exec - disabled - also list operand  - returns 1 with error output")
                {
                    And = {AppSettings = SeparatorDisabled},
                    WhenArgs = "ListPlusOne one -- a1 b2 c3",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized option '--'"}
                    }
                },
                new Given<App>("exec - enabled - without list operand - returns 1 with error output")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "NoArgs -- a1 b2 c3",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized option '--'"}
                    }
                },
                new Given<App>("exec - enabled - only list operand - extra args after separator - remaining args added to operand list ")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "list -- a1 b2 c3",
                    Then =
                    {
                        Outputs =
                        {
                            new App.ListResults {Extras = new List<string> {"a1", "b2", "c3"}}
                        }
                    }
                },
                new Given<App>("exec - enabled - only list operand - extra args before and after separator - remaining args added to operand list ")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "list a1 -- b2 c3",
                    Then =
                    {
                        Outputs =
                        {
                            new App.ListResults {Extras = new List<string> {"a1", "b2", "c3"}}
                        }
                    }
                },
                new Given<App>("exec - enabled - also list operand - extra args after separator - remaining args added to operand list ")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "ListPlusOne one -- a1 b2 c3",
                    Then =
                    {
                        Outputs =
                        {
                            new App.ListResults
                            {
                                One = "one",
                                Extras = new List<string> {"a1", "b2", "c3"}
                            }
                        }
                    }
                },
                new Given<App>("exec - enabled - also list operand - extra args before and after separator - remaining args added to operand list ")
                {
                    And = {AppSettings = SeparatorEnabledBasicHelp},
                    WhenArgs = "ListPlusOne one a1 -- b2 c3",
                    Then =
                    {
                        Outputs =
                        {
                            new App.ListResults
                            {
                                One = "one",
                                Extras = new List<string> {"a1", "b2", "c3"}
                            }
                        }
                    }
                },
            };

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void NoArgs()
            {
                TestOutputs.Capture(true);
            }

            public void List(
                [Argument]
                List<string> extras)
            {
                TestOutputs.Capture(new ListResults { Extras = extras });
            }

            public void ListPlusOne(
                [Argument]
                string one,
                [Argument]
                List<string> extras)
            {
                this.TestOutputs.Capture(new ListResults
                {
                    One = one,
                    Extras = extras
                });
            }

            public class ListResults
            {
                public string One { get; set; }
                public List<string> Extras { get; set; }
            }
        }
    }
}