using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Arguments
{
    [TestFixture]
    public class Arguments_Attributes
    {
        public class Program
        {
            // begin-snippet: arguments_attributes
            public void LaunchRocket(
                [Operand("planet", Description = "Name of the planet you wish the rocket to go")]
                string planetName,
                [Option('t', "turbo", Description = "Name of the planet you wish the rocket to go")]
                bool turbo,
                [Option('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)]
                bool abort)
            // end-snippet
            { }
        }
        public class ProgramAlt
        {
            // begin-snippet: arguments_attributes_alt
            public void LaunchRocket(
                    [Positional("planet", Description = "Name of the planet you wish the rocket to go")]
                    string planetName,
                    [Named('t', "turbo", Description = "Name of the planet you wish the rocket to go")]
                    bool turbo,
                    [Named('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)]
                    bool abort)
                // end-snippet
            { }
        }

        public static BashSnippet LaunchRocketHelp = new("arguments_attributes_help",
            new AppRunner<Program>(), "mission-control.exe", "LaunchRocket --help", 0,
            @"Usage: {0} LaunchRocket [options] <planet>

Arguments:

  planet  <TEXT>
  Name of the planet you wish the rocket to go

Options:

  -t | --turbo
  Name of the planet you wish the rocket to go

  -a | --abort  <BOOLEAN>
  Abort the launch before takeoff
  Allowed values: true, false");

        public static BashSnippet LaunchRocketHelpAlt = new("arguments_attributes_alt_help",
            new AppRunner<ProgramAlt>(), "mission-control.exe", "LaunchRocket --help", 0,
            LaunchRocketHelp.Output);

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}