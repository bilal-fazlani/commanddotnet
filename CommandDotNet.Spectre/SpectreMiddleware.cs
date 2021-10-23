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
                var console = ansiConsole ?? AnsiConsole.Console;

                c.Console = ansiConsole as IConsole ?? new AnsiConsoleForwardingConsole(console);
                c.UseParameterResolver(ctx => console);
                c.Services.Add(console);
            });
        }
    }
}