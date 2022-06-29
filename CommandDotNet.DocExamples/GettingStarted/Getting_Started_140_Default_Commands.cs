using System;
using CommandDotNet.TestTools;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
#pragma warning disable CS8618
    [TestFixture]
    public class Getting_Started_140_Default_Commands
    {
        public class AddApp
        {
            // begin-snippet: getting-started-140-default-commands
            public class Program
            {
                static int Main(string[] args)
                {
                    return new AppRunner<Program>().Run(args);
                }

                [DefaultCommand]
                public void Execute(int x, int y) => Console.WriteLine(x + y);
            }
            // end-snippet
        }
        public class CommandPerClassApp
        {
            // begin-snippet: getting-started-140-default-commands-command-per-class
            public class Program
            {
                static int Main(string[] args)
                {
                    return new AppRunner<Program>().Run(args);
                }

                [Subcommand]
                public Add Add { get; set; }

                [Subcommand]
                public Subtract Subtract { get; set; }
            }

            public class Add
            {
                [DefaultCommand]
                public void Execute(int x, int y) => Console.WriteLine(x + y);
            }

            public class Subtract
            {
                [DefaultCommand]
                public void Execute(int x, int y) => Console.WriteLine(x - y);
            }
            // end-snippet
        }

        public static BashSnippet Help = new("getting-started-140-default-commands-help",
            new AppRunner<AddApp.Program>(),
            "add.exe", "Add -h", 0,
            @"Usage: {0} <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>");

        public static BashSnippet Add = new("getting-started-140-default-commands-add",
            new AppRunner<AddApp.Program>().InterceptSystemConsoleWrites(),
            "add.exe", "40 20", 0,
            @"60");


        public static BashSnippet CommandPerClassApp_Help = new("getting-started-140-default-commands-help-command-per-class",
            new AppRunner<CommandPerClassApp.Program>(),
            "dotnet calculator.dll", "--help", 0,
            @"Usage: {0} [command]

Commands:

  Add
  Subtract

Use ""{0} [command] --help"" for more information about a command.");

        public static BashSnippet CommandPerClassApp_Help_Add = new("getting-started-140-default-commands-add-help-command-per-class",
            new AppRunner<CommandPerClassApp.Program>(),
            "dotnet calculator.dll", "Add -h", 0,
            @"Usage: {0} Add <x> <y>

Arguments:

  x  <NUMBER>

  y  <NUMBER>");

        public static BashSnippet CommandPerClassApp_Add = new("getting-started-140-default-commands-add-command-per-class",
            new AppRunner<CommandPerClassApp.Program>().InterceptSystemConsoleWrites(),
            "dotnet calculator.dll", "Add 40 20", 0,
            @"60");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}