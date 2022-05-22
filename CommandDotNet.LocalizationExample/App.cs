using Microsoft.Extensions.Configuration;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using System;
using Serilog.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using CommandDotNet.FluentValidation;
using CommandDotNet.LocalizationExample.Commands;
using CommandDotNet.LocalizationExample.Shared;

namespace CommandDotNet.LocalizationExample
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IConfigurationRoot _config;
        private readonly IStringLocalizer<LogMessages> _LogLocalizer;

        public App(ILogger<App> logger, IConfigurationRoot config, IStringLocalizer<LogMessages> logLocalizer)
        {
            _LogLocalizer = logLocalizer;
            _logger = logger;
            _config = config;
        }

        public int Run(string[] args)
        {
            string logKey = Guid.NewGuid().ToString();
            int exitcode;
            IDictionary<string, IStringLocalizer> localizers = ConfigureLocalizers();
            var appsettings = new AppSettings
            {
                Localize = t => localizers["commands"][t]
            };

            using (LogContext.PushProperty("logKey", logKey))
            {
                exitcode =  new AppRunner(
                                typeof(Menu),
                                appsettings,
                                new CoreResources((IStringLocalizer<CoreResources>)localizers["core"]))
                .UseFluentValidation(false, null, new FluentValidation.ResourcesProxy(t => localizers["fluent"][t]))
                .UseMicrosoftDependencyInjection(Program._serviceProvider)
                .UseLocalizeDirective()
                .Run(args);

                _logger.LogInformation(_LogLocalizer.GetString("ExitApp"));
            }
            return exitcode;
        }

        public static Dictionary<string, IStringLocalizer> ConfigureLocalizers()
        {
            var localizers = new Dictionary<string, IStringLocalizer>();
            localizers.Add("core", Program.GetService<IStringLocalizer<CoreResources>>());
            localizers.Add("fluent", Program.GetService<IStringLocalizer<FluentValidationResources>>());
            localizers.Add("commands", Program.GetService<IStringLocalizer<CommandResources>>());

            return localizers;
        }
    }
}
