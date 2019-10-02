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
        internal static Task<int> PromptForMissingOperands(CommandContext commandContext, ExecutionDelegate next)
        {
            var parseResult = commandContext.ParseResult;
            if (!parseResult.HelpWasRequested())
            {
                var console = commandContext.Console;
                parseResult.TargetCommand.Operands
                    .Where(a => a.RawValues.IsNullOrEmpty() && a.DefaultValue.IsNullValue())
                    .ForEach(a => a.RawValues = PromptForValues(a, console));
            }

            return next(commandContext);
        }

        private static ICollection<string> PromptForValues(Operand argument, IConsole console)
        {
            ICollection<string> inputs;
            if (argument.Arity.AllowsZeroOrMore())
            {
                console.Out.Write($"{argument.Name} ({argument.TypeInfo.DisplayName}) [separate values by space]: ");
                inputs = console.In.ReadLine()?.Split(' ');
            }
            else
            {
                console.Out.Write($"{argument.Name} ({argument.TypeInfo.DisplayName}): ");
                inputs = new List<string> { console.In.ReadLine() };
            }

            return inputs ?? new List<string>();
        }
    }
}