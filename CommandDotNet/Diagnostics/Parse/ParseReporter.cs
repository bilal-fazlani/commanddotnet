using System;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Diagnostics.Parse
{
    /// <summary>Reports the command and the source of values for all arguments</summary>
    public static class ParseReporter
    {
        /// <summary>
        /// Reports the command and the source of values for all arguments 
        /// </summary>
        public static void Report(CommandContext commandContext, 
            Action<string> writeln = null, Indent indent = null)
        {
            var command = commandContext.ParseResult.TargetCommand;

            indent = indent ?? new Indent();
            writeln = writeln ?? commandContext.Console.Out.WriteLine;

            var path = command.GetPath();
            writeln($"{indent}command: {(path.IsNullOrWhitespace() ? "(root)" : path)}");

            if (command.Operands.Any())
            {
                writeln(null);
                writeln($"{indent}arguments:");
                foreach (var operand in command.Operands)
                {
                    writeln(null);
                    PrintArg(operand, indent.Increment(), writeln);
                }
            }

            var options = command.AllOptions(includeInterceptorOptions: true, excludeHiddenOptions: true).ToList();
            if (options.Any())
            {
                writeln(null);
                writeln($"{indent}options:");
                foreach (var option in options)
                {
                    writeln(null);
                    PrintArg(option, indent.Increment(), writeln);
                }
            }
        }

        private static void PrintArg(IArgument argument, Indent indent, Action<string> writeln)
        {
            bool isObscured = argument.IsObscured();
            var indent2 = indent.Increment();

            writeln($"{indent}{argument.Name} <{argument.TypeInfo.DisplayName ?? (argument.Arity.AllowsNone() ? "Flag" : null)}>");
            writeln($"{indent2}value: {argument.Value?.ValueToString(isObscured)}");

            if (argument.InputValues?.Any() ?? false)
            {
                var pwd = isObscured ? Password.ValueReplacement : null;
                var values = argument.InputValues
                    .Select(iv => iv.Source == Constants.InputValueSources.Argument && argument.InputValues.Count == 1
                        ? $"{ValuesToString(iv, pwd)}" 
                        : $"[{iv.Source}{(iv.IsStream ? " stream" : null)}] {ValuesToString(iv, pwd)}")
                    .ToList();

                if (values.Count == 1)
                {
                    writeln($"{indent2}inputs: {values.Single()}");
                }
                else
                {
                    writeln($"{indent2}inputs:");
                    values.ForEach(v => writeln($"{indent2.Increment()}{v}"));
                }
            }
            else
            {
                writeln($"{indent2}inputs:");
            }

            if (argument.Default != null)
            {
                // don't include source when the default is defined as a parameter or property.
                // only show externally defined sources
                writeln(argument.Default.Source.StartsWith("app.")
                    ? $"{indent2}default: {argument.Default.Value.ValueToString(isObscured)}"
                    : $"{indent2}default: source={argument.Default.Source} key={argument.Default.Key}: {argument.Default.Value.ValueToString(isObscured)}");
            }
            else
            {
                writeln($"{indent2}default:");
            }
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