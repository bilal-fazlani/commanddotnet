using System.Collections.Generic;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Arguments
{
    [TestFixture]
    public class Arguments_Collections
    {
        public class Program
        {
            // begin-snippet: arguments_collections
            public void LaunchRocket(IConsole console,
                    IEnumerable<string> planets,
                    [Option('c', "crew")] string[] crew)
                // end-snippet
            {
                console.WriteLine("launching rocket");
                console.WriteLine($"planets: {string.Join(',', planets)}");
                console.WriteLine($"crew: {string.Join(',', crew)}");
            }
        }

        public class Program_PassThru
        {
            // begin-snippet: arguments_collections_argument_separator_passthru
            [Command(ArgumentSeparatorStrategy = ArgumentSeparatorStrategy.PassThru)]
            public void LaunchRocket(IConsole console, CommandContext ctx,
                    IEnumerable<string> planets,
                    [Option('c', "crew")] string[] crew)
                // end-snippet
            {
                // use Program to keep the examples consistent
                new Program().LaunchRocket(console, planets, crew);
                console.WriteLine($"separated: {string.Join(',', ctx.ParseResult!.SeparatedArguments)}");
            }
        }

        public static BashSnippet LaunchRocketHelp = new("arguments_collections_help",
            new AppRunner<Program>(), "mission-control.exe", "LaunchRocket --help", 0,
            @"Usage: {0} LaunchRocket [options] <planets>

Arguments:

  planets (Multiple)  <TEXT>

Options:

  -c | --crew (Multiple)  <TEXT>");

        public static BashSnippet LaunchRocketExe = new("arguments_collections_exe",
            new AppRunner<Program>(), 
            "mission-control.exe", "LaunchRocket mars earth jupiter -c aaron -c alex", 0,
            @"launching rocket
planets: mars,earth,jupiter
crew: aaron,alex");

        public static BashSnippet LaunchRocketExeIntermixed = new("arguments_collections_exe_intermixed",
            new AppRunner<Program>(), 
            "mission-control.exe", "LaunchRocket mars -c aaron earth -c alex jupiter", 0,
            LaunchRocketExe.Output);

        public static BashSnippet LaunchRocketExeSplit = new("arguments_collections_exe_split_args_only",
            new AppRunner<Program>(new AppSettings { Arguments = { DefaultOptionSplit = ',' } }), 
            "mission-control.exe", "LaunchRocket mars earth jupiter -c aaron,alex", 0,
            LaunchRocketExe.Output, argsOnlySnippet: true);

        public static BashSnippet LaunchRocketExeSplitDirective = new("arguments_collections_exe_split_directive_args_only",
            new AppRunner<Program>(),
            "mission-control.exe", "[split:-] LaunchRocket mars earth jupiter -c aaron-alex", 0,
            LaunchRocketExe.Output, argsOnlySnippet: true);

        public static BashSnippet LaunchRocketExeArgumentSeparator = new("arguments_collections_exe_argument_separator_passthru",
            new AppRunner<Program_PassThru>(), 
            "mission-control.exe", "LaunchRocket mars -c alex -- additional args here", 0,
            @"launching rocket
planets: mars
crew: alex
separated: additional,args,here");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}