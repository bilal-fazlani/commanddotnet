using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Parsing
{
    internal static class PipedInputMiddleware
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static AppRunner AppendPipedInput(this AppRunner appRunner)
        {
            // -1 to ensure this middleware runs before any prompting so the value won't appear null
            return appRunner.Configure(c => 
                c.UseMiddleware(InjectPipedInputToOperandList, MiddlewareSteps.PipedInput));
        }

        private static Task<int> InjectPipedInputToOperandList(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.Console.IsInputRedirected)
            {
                AssignPipedInput(ctx);
            }

            return next(ctx);
        }

        private static void AssignPipedInput(CommandContext ctx)
        {
            var pipeSymbol = ctx.Original.Tokens.TryGetDirective("pipeto", out var value)
                ? value.Split(':').Last()
                : ctx.AppConfig.AppSettings.Arguments.DefaultPipeTargetSymbol;

            if (pipeSymbol is null)
            {
                return;
            }

            // default to operand list if pipe symbol not provided.
            var command = ctx.ParseResult!.TargetCommand;
            var pipeTarget = GetPipeTarget(command, pipeSymbol)
                             ?? command.Operands.FirstOrDefault(o => o.Arity.AllowsMany());
            
            if (pipeTarget is null)
            {
                Log.DebugFormat("No list operands or pipe symbols found for {0}", ctx.ParseResult.TargetCommand.Name);
            }
            else
            {
                Log.DebugFormat("Piping input to {0}", pipeTarget.DefinitionSource);
                pipeTarget.InputValues.Add(new InputValue(Resources.A.Input_piped_lc, true, GetPipedInput(pipeTarget, ctx.Console)));
            }
        }

        private static IArgument? GetPipeTarget(Command command, string? pipeSymbol)
        {
            var args = command
                .AllArguments(includeInterceptorOptions: true)
                .Where(o => o.Arity.AllowsOneOrMore()
                            && o.InputValues.Any(iv =>
                                iv.Source == Resources.A.Common_argument_lc
                                && (iv.Values?.Any(v => v == pipeSymbol) ?? false)))
                .ToList();

            if (args.Count > 1)
            {
                var argumentNames = args.Select(a => a.Name).ToCsv(", ");
                throw new ValueParsingException(Resources.A.Input_Piped_targetted_multiple_arguments(argumentNames));
            }

            if (args.Count == 0)
            {
                return null;
            }

            var pipeTarget = args.Single();

            var inputValue = pipeTarget.InputValues
                .First(iv => iv.Source == Resources.A.Common_argument_lc);
            if (inputValue.Values!.Count() == 1)
            {
                pipeTarget.InputValues.Remove(inputValue);
            }
            else
            {
                inputValue.Values = inputValue.Values?.Where(v => v != pipeSymbol).ToList();
            }

            return pipeTarget;
        }

        public static IEnumerable<string> GetPipedInput(IArgument pipeTarget, IConsole console)
        {
            Func<string?> readLine = console.In.ReadLine;
            return pipeTarget.Arity.Maximum < int.MaxValue
                ? readLine.EnumeratePipedInput().Take(pipeTarget.Arity.Maximum)
                : readLine.EnumeratePipedInput();
        }
    }
}