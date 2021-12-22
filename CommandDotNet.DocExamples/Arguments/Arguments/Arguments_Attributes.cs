using System;
using CommandDotNet.TestTools;
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
                [Option('t', "turbo", Description = "Do you want to go fast?")]
                bool turbo,
                [Option('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)]
                bool abort)
            // end-snippet
            { Console.WriteLine($"planet={planetName} turbo={turbo} abort={abort}"); }
        }

        public class Program_NamedAndPositional
        {
            // begin-snippet: arguments_attributes_alt
            public void LaunchRocket(
                    [Positional("planet", Description = "Name of the planet you wish the rocket to go")]
                    string planetName,
                    [Named('t', "turbo", Description = "Do you want to go fast?")]
                    bool turbo,
                    [Named('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)]
                    bool abort)
                // end-snippet
            { Console.WriteLine($"planetName={planetName} turbo={turbo} abort={abort}"); }
        }

        public static BashSnippet LaunchRocket_Help = new("arguments_attributes_help",
            new AppRunner<Program>(), "mission-control.exe", "LaunchRocket --help", 0,
            @"Usage: {0} LaunchRocket [options] <planet>

Arguments:

  planet  <TEXT>
  Name of the planet you wish the rocket to go

Options:

  -t | --turbo
  Do you want to go fast?

  -a | --abort  <BOOLEAN>
  Abort the launch before takeoff
  Allowed values: true, false");

        public static BashSnippet LaunchRocket_NamedAndPositional_Help = new("arguments_attributes_alt_help",
            new AppRunner<Program_NamedAndPositional>(), "mission-control.exe", "LaunchRocket --help", 0,
            LaunchRocket_Help.Output);

        private static AppSettings appSettingsForWindows =
            // begin-snippet: AppSettings_for_windows
            new AppSettings { Parser = { AllowBackslashOptionPrefix = true } };
        // end-snippet

        public static BashSnippet LaunchRocket_Windows_Help = new("arguments_attributes_windows_help",
            new AppRunner<Program>(appSettingsForWindows), 
            "mission-control.exe", "LaunchRocket --help", 0,
            LaunchRocket_Help.Output);

        public static BashSnippet LaunchRocket_Windows_Exe = new("arguments_attributes_windows_exe",
            new AppRunner<Program>(appSettingsForWindows).InterceptSystemConsoleWrites(), 
            "mission-control.exe", "LaunchRocket /turbo /a true mars", 0,
            "planet=mars turbo=True abort=True");

        private static AppSettings appSettingsForPowerShell =
            // begin-snippet: AppSettings_for_powershell
            new AppSettings { Parser = { AllowSingleHyphenForLongNames = true } };
        // end-snippet

        public static BashSnippet LaunchRocket_Powershell_Help = new("arguments_attributes_powershell_help",
            new AppRunner<Program>(appSettingsForPowerShell),
            "mission-control.exe", "LaunchRocket --help", 0,
            LaunchRocket_Help.Output);

        public static BashSnippet LaunchRocket_Powershell_Exe = new("arguments_attributes_powershell_exe",
            new AppRunner<Program>(appSettingsForPowerShell).InterceptSystemConsoleWrites(),
            "mission-control.exe", "LaunchRocket -turbo -a true mars", 0,
            "planet=mars turbo=True abort=True");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}