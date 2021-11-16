using System;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Tokens;

namespace CommandDotNet.Diagnostics.Parse
{
    /// <summary>Reports the command and the source of values for all arguments</summary>
    public static class ParseReporter
    {
        /// <summary>
        /// Reports the command and the source of values for all arguments 
        /// </summary>
        public static void Report(
            CommandContext commandContext,
            bool includeRawCommandLine = false,
            Action<string?>? writeln = null, 
            Indent? indent = null)
        {
            if (commandContext.ParseResult == null)
            {
                return;
            }

            var command = commandContext.ParseResult.TargetCommand;

            indent ??= new Indent();
            writeln ??= s => commandContext.Console.Out.WriteLine(s);

            if (includeRawCommandLine)
            {
                writeln(Resources.A.ParseReport_Raw_command_line(commandContext.Environment.CommandLine));
            }
            
            var path = command.GetPath();
            writeln($"{indent}{Resources.A.Common_command_lc}: {(path.IsNullOrWhitespace() ? $"({Resources.A.ParseReport_root_lc})" : path)}");

            if (command.Operands.Any())
            {
                writeln("");
                writeln($"{indent}{Resources.A.Common_arguments_lc}:");
                foreach (var operand in command.Operands)
                {
                    writeln("");
                    PrintArg(operand, indent.Increment(), writeln);
                }
            }

            var options = command.AllOptions(includeInterceptorOptions: true, excludeHiddenOptions: true).ToList();
            if (options.Any())
            {
                writeln("");
                writeln($"{indent}{Resources.A.Common_options_lc}:");
                foreach (var option in options)
                {
                    writeln("");
                    PrintArg(option, indent.Increment(), writeln);
                }
            }
        }

        private static void PrintArg(IArgument argument, Indent indent, Action<string> writeln)
        {
            bool isObscured = argument.IsObscured();
            var indent2 = indent.Increment();
              
            var txtValue = Resources.A.Common_value_lc;
            var txtInputs = Resources.A.Input_inputs_lc;
            var txtDefault = Resources.A.Common_default_lc;

            var displayName = argument.TypeInfo.DisplayName.IsNullOrEmpty() 
                ? (((argument as Option)?.IsFlag ?? false) ? Resources.A.Common_Flag : null)
                : argument.TypeInfo.DisplayName;
            writeln($"{indent}{argument.Name} <{displayName}>");
            var valueString = argument.Value?.ValueToString(argument);
            writeln(valueString == null ? $"{indent2}{txtValue}:" : $"{indent2}{txtValue}: {valueString}");

            if (!argument.InputValues.IsNullOrEmpty())
            {
                var pwd = isObscured ? Password.ValueReplacement : null;
                var values = argument.InputValues
                    .Select(iv => iv.Source == Resources.A.Common_argument_lc && argument.InputValues.Count == 1
                        ? $"{ValuesToString(iv, pwd)}"
                        : iv.IsStream
                            ? $"[{iv.Source} {Resources.A.Input_stream_lc}]"
                            : $"[{iv.Source}] {ValuesToString(iv, pwd)}")
                    .ToList();

                if (values.Count == 1)
                {
                    writeln($"{indent2}{txtInputs}: {values.Single()}");
                }
                else
                {
                    writeln($"{indent2}{txtInputs}:");
                    values.ForEach(v => writeln($"{indent2.Increment()}{v}"));
                }
            }
            else
            {
                writeln($"{indent2}{txtInputs}:");
            }

            if (argument.Default != null)
            {
                // don't include source when the default is defined as a parameter or property.
                // only show externally defined sources
                writeln(argument.Default.Source.StartsWith("app.")
                    ? $"{indent2}{txtDefault}: " +
                      $"{argument.Default.Value.ValueToString(argument)}"
                    : $"{indent2}{txtDefault}: " +
                      $"{Resources.A.Common_source_lc}={argument.Default.Source} " +
                      $"{Resources.A.Common_key_lc}={argument.Default.Key}: " +
                      $"{argument.Default.Value.ValueToString(argument)}");
            }
            else
            {
                writeln($"{indent2}{txtDefault}:");
            }
        }

        private static string? ValuesToString(InputValue iv, string? pwd)
        {
            return iv.ValuesFromTokens != null
                    ? iv.ValuesFromTokens?.Select(vft => VftToString(vft, pwd)).ToCsv(", ")
                    : iv.Values?.Select(v => pwd ?? v).ToCsv(", ");
        }

        private static string VftToString(ValueFromToken vft, string? pwd)
        {
            // when the value is the original value, there's no need to show how we got it
            var supplyChain = RecurseTokens(new TokenValues(vft.ValueToken, vft.OptionToken), pwd);
            return vft.Value == supplyChain 
                ? pwd ?? vft.Value 
                : $"{pwd ?? vft.Value} ({Resources.A.Common_from_lc}: {supplyChain})";
        }

        private static string RecurseTokens(TokenValues vft, string? pwd)
        {
            if ((vft.OptionToken?.SourceToken ?? vft.ValueToken?.SourceToken) == null)
            {
                return PrettifyTokens(vft, pwd);
            }

            return vft.HasSourceToken
                ? $"{RecurseTokens(new TokenValues( vft.ValueToken?.SourceToken, vft.OptionToken?.SourceToken), pwd)}" +
                  $" -> {PrettifyTokens(vft, pwd)}"
                : PrettifyTokens(vft, pwd);
        }

        private static string PrettifyTokens(TokenValues vft, string? pwd)
        {
            return vft.OptionToken?.RawValue == vft.ValueToken?.RawValue 
            ? $"{vft.OptionToken?.RawValue}".Trim()
            : $"{vft.OptionToken?.RawValue} {pwd ?? vft.ValueToken?.RawValue}".Trim();
        }

        private class TokenValues
        {
            public Token? ValueToken { get; }
            public Token? OptionToken { get; }

            public bool HasSourceToken => ValueToken?.SourceToken is { } || OptionToken?.SourceToken is { };

            public TokenValues(Token? valueToken, Token? optionToken)
            {
                ValueToken = valueToken;
                OptionToken = optionToken;
            }
        }
    }
}