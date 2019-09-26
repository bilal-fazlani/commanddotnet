using System;
using CommandDotNet.Example.DocExamples;
using CommandDotNet.Example.Issues;
using CommandDotNet.FluentValidation;
using CommandDotNet.Help;
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

        private static int Run<TApp>(string[] args, Action<AppSettings> config = null) where TApp : class
        {
            var appSettings = new AppSettings
            {
                EnableDirectives = true,
                EnableVersionOption = true,
                Help =
                {
                    TextStyle = HelpTextStyle.Detailed
                }
            };

            config?.Invoke(appSettings);
            return new AppRunner<TApp>(appSettings)
                .UseCancellationHandlers()
                .UseDebugDirective()
                .UseParseDirective()
                .UseVersionMiddleware()
                .UseFluentValidation()
                .UsePromptForMissingOperands()
                .UseResponseFiles()
                .UseNewerReleaseAlertOnGitHub("bilal-fazlani", "commanddotnet")
                .Run(args);
        }

        private static int RunDocExample(string[] args)
        {
            return Run<SomeClass>(args, s =>
            {
                s.EnableVersionOption = false;
                // s.Help.UsageAppNameStyle = UsageAppNameStyle.GlobalTool;
                s.Case = Case.KebabCase;
            });
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
        }
    }
}