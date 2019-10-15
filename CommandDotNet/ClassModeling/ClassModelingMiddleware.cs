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
                c.UseMiddleware((context, next) => CreateRootCommand(context, next, rootCommandType), MiddlewareStages.Tokenize, int.MaxValue);
                c.UseMiddleware(AssembleInvocationPipelineMiddleware, MiddlewareStages.ParseInput);
                c.UseMiddleware(BindValuesMiddleware.BindValues, MiddlewareStages.BindValues);
                c.UseMiddleware(ResolveCommandClassesMiddleware.ResolveInstances, MiddlewareStages.BindValues);
                c.UseMiddleware(InvokeInvocationPipelineMiddleware, MiddlewareStages.Invoke, int.MaxValue);
            });
        }

        private static Task<int> CreateRootCommand(
            CommandContext commandContext, ExecutionDelegate next, Type rootCommandType)
        {
            commandContext.RootCommand = ClassCommandDef.CreateRootCommand(rootCommandType, commandContext);
            return next(commandContext);
        }

        private static Task<int> AssembleInvocationPipelineMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            var command = commandContext.ParseResult.TargetCommand;
            var commandDef = command.GetCommandDef();
            if (commandDef != null)
            {
                var pipeline = commandContext.InvocationPipeline;
                pipeline.TargetCommand = new InvocationStep
                {
                    Command = command,
                    Invocation = commandDef.InvokeMethodDef
                };
                command.GetParentCommands(includeCurrent:true)
                    .Select(cmd => (cmd, def: cmd.GetCommandDef()))
                    .Where(c => c.def != null && c.def.HasInterceptor) // in case command is defined by a different middleware
                    .Reverse()
                    .ForEach(c =>
                    {
                        pipeline.AncestorInterceptors.Add(new InvocationStep
                        {
                            Command = c.cmd,
                            Invocation = c.def.InterceptorMethodDef
                        });
                    });
            }

            return next(commandContext);
        }

        private static Task<int> InvokeInvocationPipelineMiddleware(CommandContext commandContext, ExecutionDelegate _)
        {
            Task<int> Invoke(InvocationStep step, CommandContext context, ExecutionDelegate next, bool isCommand)
            {
                var result = step.Invocation.Invoke(context, step.Instance, next);
                return isCommand 
                    ? result.GetResultCodeAsync()
                    : (Task<int>)result;
            }

            var pipeline = commandContext.InvocationPipeline;

            return pipeline.AncestorInterceptors
                .Select(i => new ExecutionMiddleware((ctx, next) => Invoke(i, ctx, next, false)))
                .Concat(new ExecutionMiddleware((ctx, next) => Invoke(pipeline.TargetCommand, ctx, next, true)).ToEnumerable())
                .InvokePipeline(commandContext);
        }
    }
}