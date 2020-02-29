using System.Linq;
using CommandDotNet.Example.DocExamples;
using CommandDotNet.Directives;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using CommandDotNet.NewerReleasesAlerts;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            return new AppRunner<Examples>()
                .UseDefaultMiddleware()
                .UseLog2ConsoleDirective()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation()
                .UseNewerReleaseAlertOnGitHub("bilal-fazlani", "commanddotnet", 
                    skipCommand: command => command.GetParentCommands(true).Any(c => c.Name == "pipes"))
                .Run(args);
        }
    }
}