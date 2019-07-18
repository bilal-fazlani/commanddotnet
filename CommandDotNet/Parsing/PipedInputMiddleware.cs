using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;

namespace CommandDotNet.Parsing
{
    internal static class PipedInputMiddleware
    {
        internal static AppBuilder EnablePipedInput(this AppBuilder appBuilder)
        {
            return appBuilder.AddMiddlewareInStage(InjectPipedInput, MiddlewareStages.PostParseInputPreBindValues);
        }

        private static Task<int> InjectPipedInput(CommandContext ctx, Func<CommandContext, Task<int>> next)
        {
            bool AcceptPipedInput(IOperand o)
            {
                return o.CustomAttributes.GetCustomAttribute<OperandAttribute>()?.AppendPipedInput ?? false;
            }

            if (ctx.Console.IsInputRedirected)
            {
                var pipedInputTargets = ctx.CurrentCommand.Operands
                    .Where(AcceptPipedInput)
                    .ToList();

                if (pipedInputTargets.Count > 1)
                {
                    ctx.Console.Error.WriteLine("only one operand can enable piped input. " +
                                                $"enabled operands:{pipedInputTargets.Select(p => p.Name).ToOrderedCsv()}");
                    return Task.FromResult(1);
                }

                if (pipedInputTargets.Count == 1)
                {
                    IOperand operand = pipedInputTargets.Single();
                    if (operand.Arity.AllowsZeroOrOne())
                    {
                        // only supporting the list operand for a command gives us a few benefits
                        // 1. there can be only one list operand per command.
                        //    no need to enforce this only one argument has EnablePipedInput=true
                        // 2. no need to handle case where a single value operand has EnablePipedInput=true
                        //    otherwise we either drop all but the first value or throw an exception
                        //    both have pros & cons
                        // 3. List operands are specified and provided last, avoiding awkward cases
                        //    where piped input is provided for arguments positioned before others.
                        //    We'd need to inject additional middleware to inject tokens in this case.
                        // 4. piped values can be merged with args passed to the command.
                        //    this can become an option passed into appBuilder.EnablePipedInput(...)
                        //    if a need arises to throw instead of merge
                        ctx.Console.Error.WriteLine($"Piped input can only be enabled for multi-value operands. `{operand.Name}` is not multi-value.");
                        return Task.FromResult(1);
                    }
                    
                    var pipedInput = ctx.ContextData.GetOrAdd(() => GetPipedInput(ctx.Console));
                    ctx.ParseResult.ArgumentValues.GetOrAdd(operand).AddRange(pipedInput);
                }
            }

            return next(ctx);
        }

        public static ICollection<string> GetPipedInput(IConsole console)
        {
            if (console.IsInputRedirected)
            {
                var input = console.In.ReadToEnd().TrimEnd('\r', '\n');
                return input
                    .Split(
                        new[] {"\r\n", "\r", "\n"},
                        StringSplitOptions.None)
                    .Select(s => s.Trim())
                    .ToArray();
            }

            return new string[0];
        }

        private class PipedInput
        {
            public List<string> Input { get; }

            public PipedInput(List<string> input)
            {
                Input = input;
            }
        }
    }
}