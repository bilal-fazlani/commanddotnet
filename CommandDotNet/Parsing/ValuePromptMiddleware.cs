using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;

namespace CommandDotNet.Parsing
{
    internal static class ValuePromptMiddleware
    {
        internal static Task<int> PromptForMissingOperands(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var command = commandContext.ParseResult.TargetCommand;
            var argumentValues = commandContext.ParseResult.ArgumentValues;
            var console = commandContext.AppSettings.Console;

            command.Operands
                .Where(a => !argumentValues.Contains(a) && a.DefaultValue.IsNullValue())
                .Select(a => new { arg = a, values = PromptForValues(a, console) })
                .Where(i => i.values.Count > 0)
                .ForEach(i => argumentValues.GetOrAdd(i.arg).AddRange(i.values));

            return next(commandContext);
        }

        private static List<string> PromptForValues(Operand argument, IConsole console)
        {
            List<string> inputs;
            if (argument.Arity.AllowsZeroOrMore())
            {
                console.Out.Write($"{argument.Name} ({argument.TypeInfo.DisplayName}) [separate values by space]: ");
                inputs = console.In.ReadLine()?.Split(' ').ToList() ?? new List<string>();
            }
            else
            {
                console.Out.Write($"{argument.Name} ({argument.TypeInfo.DisplayName}): ");
                inputs = new List<string> { console.In.ReadLine() };
            }

            return inputs;
        }
    }
}