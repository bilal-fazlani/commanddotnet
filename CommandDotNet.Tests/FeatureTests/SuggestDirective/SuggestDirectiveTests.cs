using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommandDotNet.DotNetSuggest;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;
using CommandDotNet.Tests.FeatureTests.Suggestions;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;
using static System.Environment;

namespace CommandDotNet.Tests.FeatureTests.SuggestDirective;

public class SuggestDirectiveTests
{
    public SuggestDirectiveTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }
    
    /* Test list:
     * - operands
     *   - extra operand
     * - spaces
     *   - after argument
     *   - after partial **
     * - FileInfo
     *   - file names
     * - DirectoryInfo
     *   - directory names
     * - after argument separator
     *   ? - how to know if after arg separator vs looking for options?
     * - response files
     *   - file names
     *   - directory names
     * - clubbed options
     *
     * - check position argument
     *   ? - is this used commonly?  The tests in System.CommandLine all
     *       seem to have the position as the end of the string.
     *
     * - check feature list for other considerations
     */

    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]

    [Theory]
    [InlineData(
        "command - includes subcommands, options and next operand allowed values", 
        "", "--togo/nClosed/nOpened/nOrder/nReserve")]
    [InlineData("command - invalid name", "Blah", "")]
    [InlineData("command - partial name", "Or", "Order")]
    [InlineData("subcommand", "Order ", "--juices/n--water/nBreakfast/nDinner/nLunch")]
    [InlineData("subcommand - not returned when not available", "Opened", "--togo")]
    [InlineData("option - show allowed values", "Reserve --meal", "Breakfast/nDinner/nLunch")]
    [InlineData("option - option prefix shows only options 1", "Order --", "--juices/n--water")]
    [InlineData("option - option prefix shows only options 2", "Order -", "--juices/n--water")]
    [InlineData("option - option prefix shows only options 3", "Order /", "/juices/n/water")]
    [InlineData("option - does not show already provided option", "Order --water --", "--juices")]
    [InlineData("option - does not show already provided option using short name", "Order -w --", "--juices")]
    [InlineData("option - partial name", "Order --jui", "--juices")]
    [InlineData("option - partial name with backslash", "Order /jui", "/juices")]
    [InlineData("option - partial allowed value", "Reserve --meal Br", "Breakfast")]
    [InlineData("option - trailing space", "Order --juices", "Apple/nBanana/nCherry")]
    [InlineData("operand - partial allowed value", "Op", "Opened")]
    [InlineData("typo before request for autocompletion 1", "Or --jui", "", 1)]
    [InlineData("typo before request for autocompletion 2", "Reserv --meal Br", "", 1)]
    [InlineData("typo before request for autocompletion 3", "Reserve --mea Br", "", 1)]
    public void Suggest(string scenario, string input, string expected, int exitCode = 0)
    {
        new AppRunner<DinerApp>()
            .UseSuggestDirective_Experimental()
            .Verify(new Scenario
            {
                When = { Args = $"[suggest] {input}"},
                Then =
                {
                    Output = NewLine == "/n" ? expected : expected.Replace("/n", NewLine),
                    ExitCode = exitCode
                }
            });
    }

    [Fact]
    public void Suggest_works_with_default_middleware()
    {
        var expected = "--togo/n--version/nClosed/nOpened/nOrder/nReserve";
        new AppRunner<DinerApp>()
            .UseDefaultMiddleware()
            .UseCommandLogger()
            .UseSuggestDirective_Experimental()
            .Verify(new Scenario
            {
                When = { Args = "[suggest]"},
                Then =
                {
                    Output = NewLine == "/n" ? expected : expected.Replace("/n", NewLine)
                }
            });
    }

    public class DinerApp
    {
        public enum Status{ Opened, Closed }
        
        public enum PartySize{one,two,three,four,five,six,seven,eight,nine,ten}
        
        public Task<int> Interceptor(InterceptorExecutionDelegate next, IConsole console, [Option] bool togo)
        {
            console.WriteLine("DinerApp.Interceptor");
            return Task.FromResult(0);
        }

        [DefaultCommand]
        public void Default(IConsole console, Status status)
        {
            console.WriteLine("DinerApp.Default");
        }

        public void Reserve(IConsole console,
            [Operand] PartySize partySize, [Operand] string name,
            [Operand] DateOnly date, [Operand] TimeOnly time, [Option] Meal meal)
        {
            console.WriteLine("DinerApp.Reserve");
        }

        public void Order(IConsole console,
            [Operand] Meal meal, [Operand] Main main, [Operand] Vegetable vegetable, [Operand] Fruit fruit,
            [Option('w')] bool water, [Option] Fruit juices)
        {
            console.WriteLine("DinerApp.Order");
        }
    }
}