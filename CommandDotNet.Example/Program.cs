using System.Collections.Specialized;
using CommandDotNet.Diagnostics;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using CommandDotNet.Spectre;

namespace CommandDotNet.Example
{
    public class Program
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
            return new AppRunner<Examples>(appNameForTests is null ? null : new AppSettings{Help = {UsageAppName = appNameForTests}})
                .UseDefaultMiddleware()
                .UseCommandLogger()
                .UseNameCasing(Case.KebabCase)
                .UsePrompter()
                .UseSpectreAnsiConsole()
                .UseSpectreArgumentPrompter()
                .UseLocalizeDirective()
                .UseLog2ConsoleDirective()
                .UseFluentValidation()
                .UseInteractiveMode("Example")
                .UseDefaultsFromAppSetting(appConfigSettings, includeNamingConventions: true);
        }
    }
}