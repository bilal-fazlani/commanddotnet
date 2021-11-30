using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Prompts
{
    internal static class ValuePromptMiddleware
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static AppRunner UseIPrompter(
            AppRunner appRunner,
            Func<CommandContext, IPrompter>? prompterFactory = null)
        {
            return appRunner.Configure(c =>
            {
                if (prompterFactory != null)
                {
                    c.Services.Add(prompterFactory);
                }

                c.UseParameterResolver(GetPrompter);
            });
        }

        internal static AppRunner UseArgumentPrompter(
            AppRunner appRunner,
            Func<CommandContext, IPrompter, IArgumentPrompter>? argumentPrompterFactory,
            Predicate<IArgument>? argumentFilter)
        {
            return appRunner.Configure(c =>
            {
                if (argumentPrompterFactory != null)
                {
                    c.Services.Add(argumentPrompterFactory);
                }

                argumentFilter ??= a => 
                    a.Arity.RequiresAtLeastOne() && !a.HasValueFromInputOrDefault();

                c.UseMiddleware(
                    (ctx, next) => PromptForMissingArguments(ctx, next, argumentFilter),
                    MiddlewareSteps.ValuePromptMissingArguments);
            });
        }

        private static Task<int> PromptForMissingArguments(
            CommandContext commandContext, 
            ExecutionDelegate next,
            Predicate<IArgument> argumentFilter)
        {
            var parseResult = commandContext.ParseResult;

            if (commandContext.Console.IsInputRedirected)
            {
                Log.Debug("Skipping prompts. Console does not support Console.ReadKey when Console.IsInputRedirected.");
                // avoid: System.InvalidOperationException: Cannot read keys when either application does not have a console or when console input has been redirected. Try Console.Read.
                return next(commandContext);
            }
            
            if (!commandContext.Environment.UserInteractive)
            {
                Log.Debug("Skipping prompts. Environment.UserInteractive is false.");
                return next(commandContext);
            }

            if (parseResult!.HelpWasRequested())
            {
                Log.Debug("Skipping prompts. Help was requested.");
                return next(commandContext);
            }

            var argumentPrompter = commandContext.GetArgumentPrompter();

            bool isCancellationRequested = false;

            parseResult.TargetCommand
                .AllArguments(includeInterceptorOptions: true)
                .Where(a => argumentFilter(a))
                .TakeWhile(_ => !commandContext.CancellationToken.IsCancellationRequested && !isCancellationRequested)
                .ForEach(a =>
                {
                    Log.Debug($"Prompting for {a.Name}");
                    
                    var values = argumentPrompter.PromptForArgumentValues(commandContext, a, out isCancellationRequested);
                    a.InputValues.Add(new InputValue(Resources.A.Input_prompt_lc, false, values));

                    Log.Debug($"Prompt for {a.Name} returned: {values.ToCsv()}");
                });

            return isCancellationRequested 
                ? ExitCodes.Success 
                : next(commandContext);
        }

        private static IPrompter GetPrompter(this CommandContext ctx)
        {
            // in theory, an implementation could track prompts and values
            // create only one prompter per CommandContext

            var prompter = ctx.Services.GetOrDefault<IPrompter>();
            if (prompter == null)
            {
                var prompterFactory = ctx.Services.GetOrDefault<Func<CommandContext, IPrompter>>();
                prompter = prompterFactory?.Invoke(ctx) ?? new Prompter(ctx.Console);
                ctx.Services.Add(prompter);
            }
            return prompter;
        }

        private static IArgumentPrompter GetArgumentPrompter(this CommandContext ctx)
        {
            // in theory, an implementation could track prompts and values
            // create only one prompter per CommandContext

            var argumentPrompter = ctx.Services.GetOrDefault<IArgumentPrompter>();
            if (argumentPrompter == null)
            {
                var prompterFactory = ctx.Services.GetOrDefault<Func<CommandContext, IPrompter, IArgumentPrompter>>();
                var prompter = ctx.GetPrompter();
                argumentPrompter = prompterFactory?.Invoke(ctx, prompter)
                                   ?? new ArgumentPrompter(prompter);
                ctx.Services.Add(argumentPrompter);
            }
            return argumentPrompter;
        }
    }
}