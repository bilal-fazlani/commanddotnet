using System.Collections.Specialized;
using CommandDotNet.Diagnostics;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;

namespace CommandDotNet.Example
{
    public class Program
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            var appConfigSettings = new NameValueCollection {{"notify.--retry-count", "2"}};

            return GetAppRunner(appConfigSettings, null).Run(args);
        }

        public static AppRunner GetAppRunner(NameValueCollection? appConfigSettings = null, string? appNameForTests = "example_app")
        {
            appConfigSettings ??= new NameValueCollection();
            return new AppRunner<Examples>(appNameForTests is null ? null : new AppSettings{Help = {UsageAppName = appNameForTests}})
                .UseDefaultMiddleware()
                .UsePrompter()
                .UseArgumentPrompter()
                .UseLocalizeDirective()
                .UseLog2ConsoleDirective()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation()
                .UseInteractiveMode("Example")
                .UseDefaultsFromAppSetting(appConfigSettings, includeNamingConventions: true);
        }
    }
}