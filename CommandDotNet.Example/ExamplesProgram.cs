using System.Collections.Specialized;
using System.Linq;
using CommandDotNet.Diagnostics;
using CommandDotNet.Example.DocExamples;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using CommandDotNet.Spectre;
using Humanizer;

namespace CommandDotNet.Example
{
    public class ExamplesProgram
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            var appConfigSettings = new NameValueCollection {{"notify.--retry-count", "2"}};

            return GetAppRunner(appConfigSettings, null, args).Run(args);
        }

        public static AppRunner GetAppRunner(NameValueCollection? appConfigSettings = null, string? appNameForTests = "example_app", string[]? args = null)
        {

            appConfigSettings ??= new NameValueCollection();
            var runner = new AppRunner<Examples>(appNameForTests is null ? null : new AppSettings{Help = {UsageAppName = appNameForTests}})
                .UseDefaultMiddleware()
                .UsePrompter()
                .UseSpectreAnsiConsole()
                .UseSpectreArgumentPrompter()
                .UseLocalizeDirective()
                .UseLog2ConsoleDirective()
                .UseFluentValidation()
                .UseInteractiveMode("Example")
                .UseDefaultsFromAppSetting(appConfigSettings, includeNamingConventions: true);

            bool isForTheDocs = args is not null && args.Contains(nameof(FromTheDocs));

            if (!isForTheDocs)
            {
                runner.UseNameCasing(Case.KebabCase);
            }

            return runner;
        }
    }
}