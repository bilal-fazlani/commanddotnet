using System;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Directives.Parse
{
    /// <summary>Reports the command and the source of values for all arguments</summary>
    public static class ParseReporter
    {
        /* Tests
            - no tokens
            - command
            - subcommand
            - arguments:operands,options,inherited options
            - value:single,multi
            - inputs
              - source: arg,pipe,prompt,multi
              - transformations: clubbing,response-file
              - value: single,multi
            - defaults
              - source: app,app-setting
              - value: single,multi
            - passwords not exposed
        */

        /// <summary>
        /// Reports the command and the source of values for all arguments 
        /// </summary>
        /// <param name="commandContext">the <see cref="CommandContext"/></param>
        /// <param name="writeln">when null, uses <see cref="CommandContext"/>.<see cref="CommandContext.Console"/></param>
        /// <param name="indent">the string to use for indents</param>
        /// <param name="depth">how deep to start the indents for this report</param>
        public static void Report(CommandContext commandContext, 
            Action<string> writeln = null, 
            string indent = "  ", 
            int depth = 0)
        {
            var console = commandContext.Console;
            var command = commandContext.ParseResult.TargetCommand;
            
            writeln = writeln ?? console.Out.WriteLine;
            var prefix = indent.Repeat(depth + 1);

            writeln($"command: {command.GetPath()}");
            writeln(null);

            writeln("arguments:");
            writeln(null);
            foreach (var operand in command.Operands)
            {
                PrintArg(operand, indent, s => writeln($"  {s}"));
            }
            writeln("options:");
            writeln(null);
            foreach (var option in command.AllOptions(includeInterceptorOptions: true, excludeHiddenOptions: true))
            {
                PrintArg(option, indent, s => writeln($"{prefix}{s}"));
            }
        }

        private static void PrintArg(IArgument argument, string indent, Action<string> writeln)
        {
            writeln($"{argument.Name} <{argument.TypeInfo.DisplayName ?? (argument.Arity.AllowsNone() ? "Flag" : null)}>");
            writeln($"{indent}value: {argument.Value?.ValueToString()}");

            if (argument.InputValues?.Any() ?? false)
            {
                var values = argument.InputValues
                    .Select(iv => iv.Source == Constants.InputValueSources.Argument && argument.InputValues.Count == 1
                        ? $"{ValuesToString(iv)}" 
                        : $"[{iv.Source}{(iv.IsStream ? " stream" : null)}] {ValuesToString(iv)}")
                    .ToList();

                if (values.Count == 1)
                {
                    writeln($"{indent}inputs: {values.Single()}");
                }
                else
                {
                    writeln($"{indent}inputs:");
                    values.ForEach(v => writeln($"{indent}{indent}{v}"));
                }
            }

            if (argument.Default != null)
            {
                // don't include source when the default is defined as a parameter or property.
                // only show externally defined sources
                writeln(argument.Default.Source.StartsWith("app.")
                    ? $"{indent}default: {argument.Default.Value.ValueToString()}"
                    : $"{indent}default: source={argument.Default.Source} key={argument.Default.Key}: {argument.Default.Value.ValueToString()}");
            }
            writeln(null);
        }

        private static string ValuesToString(InputValue iv)
        {
            return iv.ValuesFromTokens != null 
                ? iv.ValuesFromTokens?.Select(VftToString).ToCsv(", ") 
                : iv.Values?.ToCsv(", ");
        }

        private static string VftToString(ValueFromToken vft)
        {
            // when the value is the original value, there's no need to show how we got it
            var supplyChain = RecurseTokens(vft);
            return vft.Value == supplyChain 
                ? vft.Value 
                : $"{vft.Value} (from: {supplyChain})";
        }

        private static string RecurseTokens(ValueFromToken vft)
        {
            if ((vft.OptionToken?.SourceToken ?? vft.ValueToken?.SourceToken) == null)
            {
                return PrettifyTokens(vft);
            }

            return vft.TokensSourceToken == null
                ? PrettifyTokens(vft)
                : $"{RecurseTokens(new ValueFromToken(null, vft.ValueToken?.SourceToken, vft.OptionToken?.SourceToken))} -> {PrettifyTokens(vft)}";
        }

        private static string PrettifyTokens(ValueFromToken vft)
        {
            return vft.OptionToken?.RawValue == vft.ValueToken?.RawValue 
            ? $"{vft.OptionToken?.RawValue}".Trim()
            : $"{vft.OptionToken?.RawValue} {vft.ValueToken?.RawValue}".Trim();
        }

    }
}