using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal static class ClassModelingMiddleware
    {
        internal static AppRunner UseClassDefMiddleware(this AppRunner appRunner, Type rootCommandType)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware((context, next) => BuildMiddleware(rootCommandType, context, next), MiddlewareStages.Build);
                c.UseMiddleware(SetInvocationContextMiddleware, MiddlewareStages.ParseInput);
                c.UseMiddleware(BindValuesMiddleware.BindValues, MiddlewareStages.BindValues);
                c.UseMiddleware(ResolveInstancesMiddleware.ResolveInstances, MiddlewareStages.BindValues);
                c.UseMiddleware(InvokeCommandDefMiddleware, MiddlewareStages.Invoke, int.MaxValue);
            });
        }

        private static Task<int> BuildMiddleware(Type rootCommandType, CommandContext commandContext, ExecutionDelegate next)
        {
            commandContext.RootCommand = ClassCommandDef.CreateRootCommand(rootCommandType, commandContext);
            return next(commandContext);
        }

        private static Task<int> SetInvocationContextMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            var commandDef = commandContext.ParseResult.TargetCommand.Services.Get<ICommandDef>();
            if (commandDef != null)
            {
                var invocationContext = commandContext.InvocationContexts;
                invocationContext.TargetCommand = new InvocationContext
                {
                    Command = commandDef.Command,
                    Invocation = commandDef.InvokeMethodDef
                };
                commandDef.Command.GetParentCommands(includeCurrent:false)
                    .Where(cmd => cmd.HasInterceptor)
                    .Reverse()
                    .Select(cmd => (cmd, def: cmd.Services.Get<ICommandDef>()))
                    .Where(c => c.def != null) // in case command is defined by a different middleware
                    .ForEach(c =>
                    {
                        invocationContext.AncestorInterceptors.Add(new InvocationContext
                        {
                            Command = c.cmd,
                            Invocation = c.def.InterceptorMethodDef
                        });
                    });
            }

            return next(commandContext);
        }

        private static Task<int> InvokeCommandDefMiddleware(CommandContext commandContext, ExecutionDelegate _)
        {
            Task<int> Invoke(InvocationContext invocation, CommandContext context, ExecutionDelegate next, bool isCommand)
            {
                var result = invocation.Invocation.Invoke(context, invocation.Instance, next);
                return isCommand 
                    ? result.GetResultCodeAsync()
                    : (Task<int>)result;
            }

            var invocationContext = commandContext.InvocationContexts;
            var cmd = invocationContext.TargetCommand;

            var invocations = invocationContext.AncestorInterceptors
                .Select(i => new ExecutionMiddleware((ctx, next) => Invoke(i, ctx, next, false)))
                .Concat(new ExecutionMiddleware((ctx, next) => Invoke(cmd, ctx, next, true)).ToEnumerable());
            
            var invocationChain = invocations.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx, c => c.AppConfig.CancellationToken.IsCancellationRequested
                            ? Task.FromResult(0)
                            : second(c, next)));

            return invocationChain(commandContext, ctx => Task.FromResult(0));
        }
    }
}