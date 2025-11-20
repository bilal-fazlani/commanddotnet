using CommandDotNet.Spectre;
using CommandDotNet.Spectre.Testing;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.Spectre;

public class AnsiTestConsoleTests
{
    public AnsiTestConsoleTests(ITestOutputHelper output) => Ambient.Output = output;

    [Fact]
    public void AnsiTestConsole_works_with_TestTools()
    {
        new AppRunner<App>()
            .UseSpectreAnsiConsole(new AnsiTestConsole())
            .Verify(new Scenario
            {
                When = { Args = "Ansi lala" },
                Then = { Output = @"lala
" }
            });
    }

    [Fact]
    public void DuplexTextWriter_ToString_returns_captured_output_not_type_name()
    {
        // Unit test: Verify DuplexTextWriter.ToString() returns content, not "CommandDotNet.Rendering.DuplexTextWriter"
        var testConsole = new AnsiTestConsole();
        new AppRunner<App>()
            .UseSpectreAnsiConsole(testConsole)
            .RunInMem("ConsoleOut hello");

        var outWriterString = testConsole.Out.ToString();
        outWriterString.Should().NotBeNullOrEmpty();
        outWriterString.Should().NotContain("DuplexTextWriter");
        outWriterString.Should().Contain("hello");
    }

    [Fact]
    public void AnsiTestConsole_captures_IAnsiConsole_output()
    {
        // IAnsiConsole writes only appear once (correct behavior)
        new AppRunner<App>()
            .UseSpectreAnsiConsole(new AnsiTestConsole())
            .Verify(new Scenario
            {
                When = { Args = "Ansi test" },
                Then = { Output = @"test
" }
            });
    }

    [Fact]
    public void AnsiTestConsole_captures_Console_Out_output()
    {
        // Console.Out writes go through DuplexTextWriter to Spectre, captured once
        new AppRunner<App>()
            .UseSpectreAnsiConsole(new AnsiTestConsole())
            .Verify(new Scenario
            {
                When = { Args = "ConsoleOut hello" },
                Then = { Output = @"hello
" }
            });
    }

    [Fact]
    public void AnsiTestConsole_captures_Spectre_markup_as_plain_text()
    {
        // Spectre markup is rendered to plain text when EmitAnsiSequences is false (default)
        new AppRunner<App>()
            .UseSpectreAnsiConsole(new AnsiTestConsole())
            .Verify(new Scenario
            {
                When = { Args = "Markup" },
                Then = { Output = @"This is red text
This is bold text
" }
            });
    }

    [Fact]
    public void AnsiTestConsole_captures_Spectre_markup_with_ANSI_codes()
    {
        // With EmitAnsiSequences enabled, output contains ANSI escape codes:
        // \x1b[38;5;9m = red foreground (256-color mode)
        // \x1b[0m = reset all attributes
        // \x1b[1m = bold
        var testConsole = new AnsiTestConsole().EmitAnsiSequences();
        new AppRunner<App>()
            .UseSpectreAnsiConsole(testConsole)
            .Verify(new Scenario
            {
                When = { Args = "Markup" },
                Then = { Output = "\x1b[38;5;9mThis is red text\x1b[0m\n\x1b[1mThis is bold text\x1b[0m\n" }
            });
    }

    [Fact]
    public void AnsiTestConsole_captures_mixed_Console_and_Spectre_output()
    {
        // Both IAnsiConsole and Console.Out writes go to Spectre, appearing once in order
        new AppRunner<App>()
            .UseSpectreAnsiConsole(new AnsiTestConsole())
            .Verify(new Scenario
            {
                When = { Args = "Mixed" },
                Then = { Output = @"From Spectre
From Console.Out
" }
            });
    }

    private class App
    {
        public void Ansi(IAnsiConsole ansiConsole, string text)
        {
            ansiConsole.WriteLine(text);
        }

        public void ConsoleOut(IConsole console, string text)
        {
            console.Out.WriteLine(text);
        }

        public void Markup(IAnsiConsole ansiConsole)
        {
            ansiConsole.MarkupLine("[red]This is red text[/]");
            ansiConsole.MarkupLine("[bold]This is bold text[/]");
        }

        public void Mixed(IAnsiConsole ansiConsole, IConsole console)
        {
            ansiConsole.WriteLine("From Spectre");
            console.Out.WriteLine("From Console.Out");
        }
    }
}