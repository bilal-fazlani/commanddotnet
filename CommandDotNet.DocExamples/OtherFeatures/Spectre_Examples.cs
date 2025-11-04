using Spectre.Console;

namespace CommandDotNet.DocExamples.OtherFeatures;

public static class Spectre_Examples
{
    // begin-snippet: spectre_ansi_console_usage
    public class Calculator
    {
        public void Sum(IAnsiConsole console, int x, int y)
        {
            var result = x + y;
            console.MarkupLine($"[green]{x}[/] + [green]{y}[/] = [bold yellow]{result}[/]");
        }
    }
    // end-snippet
}
