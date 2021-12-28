using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Values
{
    [TestFixture]
    public class Argument_Separator
    {
        public class Program
        {
            public static AppRunner AppRunner => new AppRunner<Program>();

            public static AppRunner AppRunner_IgnoreUnexpectedOperands => new AppRunner<Program>(
                // begin-snippet: AppSettings_IgnoreUnexpectedOperands
                new AppSettings { Parser = { IgnoreUnexpectedOperands = true } }
                // end-snippet
            );

            // begin-snippet: argument_separator_end_of_options
            public void EndOfOptions(IConsole console, CommandContext ctx, string? arg1)
            {
                var parserSettings = ctx.AppConfig.AppSettings.Parser;
                console.WriteLine("IgnoreUnexpectedOperands: " + 
                                  parserSettings.IgnoreUnexpectedOperands);
                console.WriteLine("DefaultArgumentSeparatorStrategy: " + 
                                  parserSettings.DefaultArgumentSeparatorStrategy);
                console.WriteLine();
                console.WriteLine($"arg1: {arg1}");
                console.WriteLine($"separated: {string.Join(',', ctx.ParseResult!.SeparatedArguments)}");
                console.WriteLine($"remaining: {string.Join(',', ctx.ParseResult!.RemainingOperands)}");
            }
            // end-snippet

            // begin-snippet: argument_separator_pass_thru
            [Command(ArgumentSeparatorStrategy = ArgumentSeparatorStrategy.PassThru)]
            public void PassThru(IConsole console, CommandContext ctx, string? arg1)
            {
                var parserSettings = ctx.AppConfig.AppSettings.Parser;
                console.WriteLine("IgnoreUnexpectedOperands: " +
                                  parserSettings.IgnoreUnexpectedOperands);
                console.WriteLine("DefaultArgumentSeparatorStrategy: " +
                                  parserSettings.DefaultArgumentSeparatorStrategy);
                console.WriteLine();
                console.WriteLine($"arg1: {arg1}");
                console.WriteLine($"separated: {string.Join(',', ctx.ParseResult!.SeparatedArguments)}");
                console.WriteLine($"remaining: {string.Join(',', ctx.ParseResult!.RemainingOperands)}");
            }
            // end-snippet

            // begin-snippet: CommandAttribute_IgnoreUnexpectedOperands
            [Command(IgnoreUnexpectedOperands = true)]
            // end-snippet
            public void Ignore(){ }
        }

        public static BashSnippet EndOfOptions_OptionsMask_NoSeparator = new("argument_separator_end_of_options_option_mask_no_separator",
            Program.AppRunner, "example.exe", "EndOfOptions --option-mask", 1,
            @"Unrecognized option '--option-mask'

Usage: example.exe EndOfOptions [<arg1>]

Arguments:

  arg1  <TEXT>");

        public static BashSnippet EndOfOptions_OptionsMask_Separator = new("argument_separator_end_of_options_option_mask_separator",
            Program.AppRunner, "example.exe", "EndOfOptions -- --option-mask", 0,
            @"IgnoreUnexpectedOperands: False
DefaultArgumentSeparatorStrategy: EndOfOptions

arg1: --option-mask
separated: --option-mask
remaining:");

        public static BashSnippet EndOfOptions_Unexpected_Operand = new("argument_separator_end_of_options_unexpected_operand",
            Program.AppRunner, "example.exe", "EndOfOptions expected unexpected", 1,
            @"Unrecognized command or argument 'unexpected'

Usage: example.exe EndOfOptions [<arg1>]

Arguments:

  arg1  <TEXT>");

        public static BashSnippet EndOfOptions_Unexpected_Operand_Ignored = new("argument_separator_end_of_options_unexpected_operand_ignored",
            Program.AppRunner_IgnoreUnexpectedOperands, "example.exe", "EndOfOptions expected unexpected", 0,
            @"IgnoreUnexpectedOperands: True
DefaultArgumentSeparatorStrategy: EndOfOptions

arg1: expected
separated: 
remaining: unexpected");


        public static BashSnippet PassThru_Help = new("argument_separator_pass_thru_help",
            Program.AppRunner, "example.exe", "PassThru -h", 0,
            @"Usage: example.exe PassThru [<arg1>] [[--] <arg>...]

Arguments:

  arg1  <TEXT>");

        public static BashSnippet PassThru_OptionsMask_Separator = new("argument_separator_pass_thru_option_mask_separator",
            Program.AppRunner, "example.exe", "PassThru expected -- pass-thru", 0,
            @"IgnoreUnexpectedOperands: False
DefaultArgumentSeparatorStrategy: EndOfOptions

arg1: expected
separated: pass-thru
remaining:");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}