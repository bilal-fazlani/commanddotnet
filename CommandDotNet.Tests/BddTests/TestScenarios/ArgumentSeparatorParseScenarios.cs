using System.Collections.Generic;
using CommandDotNet.Models;
using CommandDotNet.Tests.BddTests.Apps;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class ArgumentSeparatorParseScenarios: ScenariosBaseTheory
    {
        private readonly AppSettings _argSeparatorEnabled = new AppSettings {AllowArgumentSeparator = true};
        private readonly AppSettings _argSeparatorDisabled = new AppSettings {AllowArgumentSeparator = false};

        public override Scenarios Scenarios =>
            new Scenarios
            {
                // TODO: these scenarios need better names after this is working
                new Given<ArgSamplesApp>("list _argSeparatorDisabled 1")
                {
                    And = {AppSettings = _argSeparatorDisabled},
                    WhenArgs = "list -- a1 b2 c3",
                    Then = {HelpContainsTexts = {"Unrecognized option '--'"}}
                },
                new Given<ArgSamplesApp>("list _argSeparatorEnabled 1")
                {
                    And = {AppSettings = _argSeparatorEnabled},
                    WhenArgs = "list -- a1 b2 c3",
                    Then = { Outputs =
                    {
                        new ArgSamplesApp.ListResults{Extras = new List<string>{"a1", "b2", "c3"}}
                    }}
                },
                new Given<ArgSamplesApp>("ListPlusOne _argSeparatorDisabled 1")
                {
                    And = {AppSettings = _argSeparatorDisabled},
                    WhenArgs = "ListPlusOne one -- a1 b2 c3",
                    Then = {HelpContainsTexts = {"Unrecognized option '--'"}}
                },
                new Given<ArgSamplesApp>("ListPlusOne _argSeparatorEnabled 1")
                {
                    And = {AppSettings = _argSeparatorEnabled},
                    WhenArgs = "ListPlusOne one -- a1 b2 c3",
                    Then = { Outputs =
                    {
                        new ArgSamplesApp.ListResults
                        {
                            One = "one",
                            Extras = new List<string>{"a1", "b2", "c3"}
                        }
                    }}
                }
            };
    }
}