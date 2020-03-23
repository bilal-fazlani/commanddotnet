using System;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Directives.Parse
{
    /// <summary>Reports the command and the source of values for all arguments</summary>
    public static class ParseReporter
    {
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

            if (command.Operands.Any())
            {
                writeln("arguments:");
                writeln(null);
                foreach (var operand in command.Operands)
                {
                    PrintArg(operand, indent, s => writeln($"  {s}"));
                }
            }

            var options = command.AllOptions(includeInterceptorOptions: true, excludeHiddenOptions: true).ToList();
            if (options.Any())
            {
                writeln("options:");
                writeln(null);
                foreach (var option in options)
                {
                    PrintArg(option, indent, s => writeln($"{prefix}{s}"));
                }
            }
        }

        private static void PrintArg(IArgument argument, string indent, Action<string> writeln)
        {
            bool isPassword = argument.TypeInfo.UnderlyingType == typeof(Password);

            writeln($"{argument.Name} <{argument.TypeInfo.DisplayName ?? (argument.Arity.AllowsNone() ? "Flag" : null)}>");
            writeln($"{indent}value: {argument.Value?.ValueToString(isPassword)}");

            if (argument.InputValues?.Any() ?? false)
            {
                var pwd = isPassword ? Password.ValueReplacement : null;
                var values = argument.InputValues
                    .Select(iv => iv.Source == Constants.InputValueSources.Argument && argument.InputValues.Count == 1
                        ? $"{ValuesToString(iv, pwd)}" 
                        : $"[{iv.Source}{(iv.IsStream ? " stream" : null)}] {ValuesToString(iv, pwd)}")
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
            else
            {
                writeln($"{indent}inputs:");
            }

            if (argument.Default != null)
            {
                // don't include source when the default is defined as a parameter or property.
                // only show externally defined sources
                writeln(argument.Default.Source.StartsWith("app.")
                    ? $"{indent}default: {argument.Default.Value.ValueToString(isPassword)}"
                    : $"{indent}default: source={argument.Default.Source} key={argument.Default.Key}: {argument.Default.Value.ValueToString(isPassword)}");
            }
            else
            {
                writeln($"{indent}default:");
            }

            writeln(null);
        }

        private static string ValuesToString(InputValue iv, string pwd)
        {
            return iv.ValuesFromTokens != null
                    ? iv.ValuesFromTokens?.Select(vft => VftToString(vft, pwd)).ToCsv(", ")
                    : iv.Values?.Select(v => pwd ?? v).ToCsv(", ");
        }

        private static string VftToString(ValueFromToken vft, string pwd)
        {
            // when the value is the original value, there's no need to show how we got it
            var supplyChain = RecurseTokens(vft, pwd);
            return vft.Value == supplyChain 
                ? pwd ?? vft.Value 
                : $"{pwd ?? vft.Value} (from: {supplyChain})";
        }

        private static string RecurseTokens(ValueFromToken vft, string pwd)
        {
            if ((vft.OptionToken?.SourceToken ?? vft.ValueToken?.SourceToken) == null)
            {
                return PrettifyTokens(vft, pwd);
            }

            return vft.TokensSourceToken == null
                ? PrettifyTokens(vft, pwd)
                : $"{RecurseTokens(new ValueFromToken(null, vft.ValueToken?.SourceToken, vft.OptionToken?.SourceToken), pwd)}" +
                  $" -> {PrettifyTokens(vft, pwd)}";
        }

        private static string PrettifyTokens(ValueFromToken vft, string pwd)
        {
            return vft.OptionToken?.RawValue == vft.ValueToken?.RawValue 
            ? $"{vft.OptionToken?.RawValue}".Trim()
            : $"{vft.OptionToken?.RawValue} {pwd ?? vft.ValueToken?.RawValue}".Trim();
        }

    }
}