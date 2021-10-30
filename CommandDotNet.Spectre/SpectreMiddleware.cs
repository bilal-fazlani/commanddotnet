using System;
using CommandDotNet.Prompts;
using CommandDotNet.Rendering;
using Spectre.Console;

namespace CommandDotNet.Spectre
{
    public static class SpectreMiddleware
    {
        public static AppRunner UseSpectreAnsiConsole(this AppRunner appRunner, IAnsiConsole? ansiConsole = null)
        {
            return appRunner.Configure(c =>
            {
                ansiConsole ??= AnsiConsole.Console;

                c.Console = ansiConsole as IConsole ?? new AnsiConsoleForwardingConsole(ansiConsole);
                c.UseParameterResolver(ctx => ansiConsole);
                c.Services.Add(ansiConsole);
            });
        }

        public static AppRunner UseSpectreToPromptForMissingArguments(this AppRunner appRunner,
            int defaultPageSize = 10,
            Func<CommandContext, IArgument, string>? argumentPromptTextOverride = null,
            Predicate<IArgument>? argumentFilter = null)
        {
            return appRunner.Configure(c =>
            {
                if (!c.Services.Contains<IAnsiConsole>())
                {
                    throw new InvalidConfigurationException(
                        $"must register {nameof(UseSpectreAnsiConsole)} to ensure an {nameof(IAnsiConsole)} is available.");
                }
                c.Services.Add<IArgumentPrompter>(new SpectreArgumentPrompter(defaultPageSize, argumentPromptTextOverride));
                appRunner.UseArgumentPrompter(argumentFilter: argumentFilter);
            });
        }
    }
}