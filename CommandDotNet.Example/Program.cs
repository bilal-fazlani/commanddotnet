using CommandDotNet.Example.DocExamples;
using CommandDotNet.Example.Issues;
using CommandDotNet.FluentValidation;
using CommandDotNet.Help;
using CommandDotNet.NameCasing;
using CommandDotNet.NewerReleasesAlerts;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            // return RunDocExample(args);
            // return Run<GitApplication>(args);
            return Run<Examples>(args);
        }

        private static int Run<TApp>(string[] args, Case @case = Case.DontChange) where TApp : class
        {
            var appSettings = new AppSettings
            {
                EnableDirectives = true,
                Help =
                {
                    TextStyle = HelpTextStyle.Detailed
                }
            };

            return new AppRunner<TApp>(appSettings)
                .UseNameCasing(@case)
                .UseVersionMiddleware()
                .UseCancellationHandlers()
                .UseDebugDirective()
                .UseParseDirective()
                .UseFluentValidation()
                .UsePrompting()
                .UseResponseFiles()
                .AppendPipedInputToOperandList()
                //.UseNewerReleaseAlertOnGitHub("bilal-fazlani", "commanddotnet")
                .Run(args);
        }

        private static int RunDocExample(string[] args)
        {
            return Run<SomeClass>(args, Case.KebabCase);
        }

        public class Examples
        {
            [SubCommand]
            public GitApplication GitApplication { get; set; }

            [SubCommand]
            public ModelApp ModelApp { get; set; }

            [SubCommand]
            public MyApplication MyApplication { get; set; }

            [SubCommand]
            public IssueApps IssueApps { get; set; }

            [SubCommand]
            public PipesApp PipesApp { get; set; }

            [SubCommand]
            public CancelMeApp CancelMeApp { get; set; }

            [SubCommand]
            public PromptApp PromptApp { get; set; }
        }
    }
}