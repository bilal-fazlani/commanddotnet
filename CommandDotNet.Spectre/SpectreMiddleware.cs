using System;
using CommandDotNet.Prompts;
using Spectre.Console;

namespace CommandDotNet.Spectre
{
    public static class SpectreMiddleware
    {
        /// <summary>
        /// Override the resources to localize or change the text used in the <see cref="SpectreArgumentPrompter"/>
        /// </summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> instance</param>
        /// <param name="resourcesOverride">
        /// The resources to use. If this method is not called,
        /// <see cref="AppSettings"/>.<see cref="AppSettings.Localize"/> will be used if available.</param>
        /// <returns></returns>
        public static AppRunner UseSpectreResources(this AppRunner appRunner, Resources resourcesOverride)
        {
            return appRunner.Configure(c =>
            {
                c.Services.GetOrCreate<SpectreAppConfig>().ResourcesSet = true;
                Resources.A = resourcesOverride;
            });
        }

        /// <summary>
        /// Makes the <see cref="IAnsiConsole"/> available as a command parameter and will
        /// forward <see cref="IConsole"/>.Out to the <see cref="IAnsiConsole"/>.
        /// </summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> instance</param>
        /// <param name="ansiConsole">
        /// optionally, the <see cref="IAnsiConsole"/> to use,
        /// else will use <see cref="AnsiConsole"/>.<see cref="AnsiConsole.Console"/>
        /// </param>
        /// <returns></returns>
        public static AppRunner UseSpectreAnsiConsole(this AppRunner appRunner, IAnsiConsole? ansiConsole = null)
        {
            return appRunner.Configure(c =>
            {
                ansiConsole ??= AnsiConsole.Console;

                c.Console = ansiConsole as IConsole ?? new AnsiConsoleForwardingConsole(ansiConsole);
                c.UseParameterResolver(_ => ansiConsole);
                c.Services.Add(ansiConsole);

                EnsureResourcesAreSet(appRunner, c);
            });
        }

        /// <summary>
        /// Adds support for prompting arguments.<br/>
        /// By default, prompts for arguments missing a required value.<br/>
        /// Missing is determined by <see cref="IArgumentArity"/>, not by any validation frameworks.
        /// </summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> instance</param>
        /// <param name="pageSize">the page size for selection lists.</param>
        /// <param name="getPromptTextCallback">Used to customize the generation of the prompt text.</param>
        /// <param name="argumentFilter">
        /// Filter the arguments that will be prompted. i.e. Create a [PromptWhenMissing] attribute, or only prompt for operands.<br/>
        /// Default filter includes only arguments where <see cref="IArgumentArity"/>.<see cref="IArgumentArity.Minimum"/> is greater than zero.
        /// </param>
        /// <returns></returns>
        public static AppRunner UseSpectreArgumentPrompter(this AppRunner appRunner,
            int pageSize = 10,
            Func<CommandContext, IArgument, string>? getPromptTextCallback = null,
            Predicate<IArgument>? argumentFilter = null)
        {
            return appRunner.Configure(c =>
            {
                if (!c.Services.Contains<IAnsiConsole>())
                {
                    throw new InvalidConfigurationException(
                        $"must register {nameof(UseSpectreAnsiConsole)} to ensure an {nameof(IAnsiConsole)} is available.");
                }
                c.Services.Add<IArgumentPrompter>(new SpectreArgumentPrompter(pageSize, getPromptTextCallback));
                appRunner.UseArgumentPrompter(argumentFilter: argumentFilter);

                EnsureResourcesAreSet(appRunner, c);
            });
        }

        private static void EnsureResourcesAreSet(AppRunner appRunner, AppConfigBuilder c)
        {
            var localizationAppSettings = appRunner.AppSettings.Localization;
            if (localizationAppSettings.Localize != null)
            {
                var config = c.Services.GetOrCreate<SpectreAppConfig>();
                if (!config.ResourcesSet)
                {
                    Resources.A = new ResourcesProxy(
                        localizationAppSettings.Localize, 
                        localizationAppSettings.UseMemberNamesAsKeys);
                    config.ResourcesSet = true;
                }
            }
        }
        private class SpectreAppConfig
        {
            public bool ResourcesSet { get; set; }
        }
    }
}