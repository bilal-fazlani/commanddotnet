using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Prompts
{
    internal static class ValuePromptMiddleware
    {
        internal static readonly int MissingArgumentPromptsOrderWithinStage = int.MaxValue - 100;

        internal static AppRunner UsePrompting(
            AppRunner appRunner,
            Func<CommandContext, IPrompter> prompterOverride = null,
            bool skipForMissingArguments = false,
            Func<CommandContext, IArgument, string> argumentPromptTextOverride = null,
            Predicate<IArgument> argumentFilter = null)
        {
            return appRunner.Configure(c =>
            {

                prompterOverride = prompterOverride
                                   ?? c.Services.Get<Func<CommandContext, IPrompter>>()
                                   ?? (ctx =>
                                   {
                                       // create only one prompter per CommandContext
                                       // in theory, an implementation could track prompts and values
                                       var prompter = ctx.Services.Get<IPrompter>();
                                       if(prompter == null)
                                       {
                                           prompter = new Prompter(ctx.Console);
                                           ctx.Services.AddOrUpdate(prompter);
                                       }
                                       return prompter;
                                   });

                c.UseParameterResolver(ctx => prompterOverride(ctx));

                if (!skipForMissingArguments)
                {
                    c.UseMiddleware(
                        (ctx, next) => PromptForMissingArguments(ctx, next,
                            new ArgumentPrompter(prompterOverride(ctx), argumentPromptTextOverride), argumentFilter), 
                        MiddlewareStages.PostParseInputPreBindValues, MissingArgumentPromptsOrderWithinStage);
                }
            });
        }

        private static Task<int> PromptForMissingArguments(
            CommandContext commandContext, 
            ExecutionDelegate next, 
            IArgumentPrompter argumentPrompter,
            Predicate<IArgument> argumentFilter)
        {
            var parseResult = commandContext.ParseResult;
            
            if (!parseResult.HelpWasRequested())
            {
                bool isCancellationRequested = false;

                parseResult.TargetCommand
                    .AllArguments(includeInterceptorOptions: true)
                    .Where(a => a.SwitchFunc(
                        operand => true, 
                        option => !option.Arity.AllowsNone() // exclude flag options: help, version, ...
                        ))
                    .Where(a => argumentFilter == null || argumentFilter(a))
                    .Where(a => a.InputValues.IsEmpty() && a.DefaultValue.IsNullValue())
                    .TakeWhile(a => !commandContext.AppConfig.CancellationToken.IsCancellationRequested && !isCancellationRequested)
                    .ForEach(a =>
                    {
                        var values = argumentPrompter.PromptForArgumentValues(commandContext, a, out isCancellationRequested);
                        a.InputValues.Add(new InputValue(Constants.InputValueSources.Prompt, values));
                    });

                if (isCancellationRequested)
                {
                    return Task.FromResult(0);
                }
            }

            return next(commandContext);
        }
    }
}