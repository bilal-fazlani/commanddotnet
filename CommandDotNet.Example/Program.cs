using System.Collections.Specialized;
using CommandDotNet.Diagnostics;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using CommandDotNet.ReadLineRepl;

namespace CommandDotNet.Example
{
    public class Program
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            var appSettings = new NameValueCollection {{"notify.--retry-count", "2"}};

            return GetAppRunner(appSettings).Run(args);
        }

        public static AppRunner GetAppRunner(NameValueCollection? appSettings = null)
        {
            appSettings ??= new NameValueCollection();
            return new AppRunner<Examples>()
                .UseDefaultMiddleware()
                .UseLog2ConsoleDirective()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation()
                .UseRepl(replConfig: new ReplConfig {AppName = "Example", ReplOption = {LongName = "interactive", ShortName = 'i'}})
                .UseDefaultsFromAppSetting(appSettings, includeNamingConventions: true);
        }
    }
}