using System.Linq;
using CommandDotNet.Example.DocExamples;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using CommandDotNet.NewerReleasesAlerts;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            return new AppRunner<RocketLauncher>()
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