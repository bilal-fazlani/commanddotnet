using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class GeneralHelpTests
    {
        public GeneralHelpTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void QuestionMark_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("-?".SplitArgs());
            result.CommandContext.ShowHelpOnExit.Should().BeTrue();
        }

        [Fact]
        public void ShortName_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("-h".SplitArgs());
            result.CommandContext.ShowHelpOnExit.Should().BeTrue();
        }

        [Fact]
        public void LongName_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("--help".SplitArgs());
            result.CommandContext.ShowHelpOnExit.Should().BeTrue();
        }

        [Fact]
        public void Help_localizes_command_and_arg_help_texts()
        {
            string? loc(string arg)
            {
                switch (arg)
                {
                    case "cmd-desc": return "aaa";
                    case "cmd-usg": return "bbb";
                    case "opn1-desc": return "ccc";
                    case "opt1-desc": return "ddd";
                    case "cmd-eht": return "eee";
                    default: return arg;
                }
            }

            new AppRunner<App>(new AppSettings {Localization = { Localize = loc }})
                .AfterRun(r => Resources.A = new Resources())
                .Verify(new Scenario
                {
                    When = { Args = "Do --help" },
                    Then = { Output = @"aaa

Usage: bbb

Arguments:

  opn1  <TEXT>
  ccc

Options:

  --opt1  <TEXT>
  ddd

eee" }
                });
        }

        private class App
        {
            [Command(Description = "cmd-desc", Usage = "cmd-usg", ExtendedHelpText = "cmd-eht")]
            public void Do(
                [Option(Description = "opt1-desc")] string opt1,
                [Operand(Description = "opn1-desc")] string opn1)
            {
            }
        }
    }
}