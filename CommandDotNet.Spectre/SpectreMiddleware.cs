using Spectre.Console;

namespace CommandDotNet.Spectre
{
    public static class SpectreMiddleware
    {
        public static AppRunner UseSpectreAnsiConsole(this AppRunner appRunner, IAnsiConsole? ansiConsole = null)
        {
            return appRunner.Configure(c =>
            {
                IAnsiConsole console;
                if (ansiConsole is null)
                {
                    console = AnsiConsole.Console;
                }
                else
                {
                    AnsiConsole.Console = console = ansiConsole;
                }

                c.Console = new AnsiConsoleForwardingConsole(console);
                c.UseParameterResolver(ctx => console);
                c.Services.Add(console);
            });
        }
    }
}