using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;

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
                c.UseMiddleware(BindValuesMiddleware, MiddlewareStages.BindValues);
                c.UseMiddleware(ResolveInstancesMiddleware, MiddlewareStages.BindValues);
                c.UseMiddleware(InvokeCommandDefMiddleware, MiddlewareStages.Invoke, int.MaxValue);
            });
        }

        private static Task<int> BuildMiddleware(Type rootCommandType, CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            commandContext.RootCommand = ClassCommandDef.CreateRootCommand(rootCommandType, commandContext);
            return next(commandContext);
        }

        private static Task<int> SetInvocationContextMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.ParseResult.TargetCommand.Services.Get<ICommandDef>();
            if (commandDef != null)
            {
                commandContext.InvocationContext.CommandInvocation = commandDef.InvokeMethodDef;
            }

            return next(commandContext);
        }

        private static Task<int> BindValuesMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.ParseResult.TargetCommand.Services.Get<ICommandDef>();
            if (commandDef != null)
            {
                var console = commandContext.Console;
                var argumentValues = commandContext.ParseResult.ArgumentValues;
                var parserFactory = new ParserFactory(commandContext.AppConfig.AppSettings);

                var middlewareArgs = commandDef.MiddlewareMethodDef.ArgumentDefs;
                var invokeArgs = commandDef.InvokeMethodDef.ArgumentDefs;

                foreach (var argumentDef in middlewareArgs.Union(invokeArgs))
                {
                    if (argumentValues.TryGetValues(argumentDef.Argument, out var values))
                    {
                        var parser = parserFactory.CreateInstance(argumentDef.Argument);
                        object value;
                        try
                        {
                            value = parser.Parse(argumentDef.Argument, values);
                        }
                        catch (ValueParsingException ex)
                        {
                            console.Error.WriteLine(ex.Message);
                            console.Error.WriteLine();
                            return Task.FromResult(2);
                        }
                        argumentDef.SetValue(value);
                    }
                    else if (argumentDef.HasDefaultValue)
                    {
                        argumentDef.SetValue(argumentDef.DefaultValue);
                    }
                }
            }
            return next(commandContext);
        }

        private static async Task<int> ResolveInstancesMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var command = commandContext.ParseResult.TargetCommand;
            var commandDef = command.Services.Get<ICommandDef>();

            bool instanceOwnedByThisMiddleware = false;

            if (commandDef != null)
            {
                var classType = commandDef.CommandHostClassType;
                var resolver = commandContext.AppConfig.DependencyResolver;

                if (resolver != null && resolver.TryResolve(classType, out var instance))
                {
                    // don't dispose an instance owned by a container
                    commandContext.InvocationContext.Instance = instance;
                }
                else
                {
                    instanceOwnedByThisMiddleware = true;
                    commandContext.InvocationContext.Instance = Activator.CreateInstance(classType);
                }
            }

            try
            {
                return await next(commandContext);
            }
            finally
            {
                if (instanceOwnedByThisMiddleware && commandContext.InvocationContext.Instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private static Task<int> InvokeCommandDefMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var ctx = commandContext.InvocationContext;
            var commandDef = commandContext.ParseResult.TargetCommand.Services.Get<ICommandDef>();

            if (commandDef.MiddlewareMethodDef != null)
            {
                return commandDef.MiddlewareMethodDef.InvokeAsMiddleware(commandContext, ctx.Instance,
                    commandCtx => ctx.CommandInvocation.Invoke(commandContext, ctx.Instance).GetResultCodeAsync());
            }

            return ctx.CommandInvocation.Invoke(commandContext, ctx.Instance).GetResultCodeAsync();
        }
    }
}