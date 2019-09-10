using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Rendering;

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
                c.UseMiddleware(ResolveInstancesMiddleware, MiddlewareStages.BindValues);
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
                commandContext.InvocationContext.CommandInvocation = commandDef.InvokeMethodDef;
                if (commandDef.InterceptorMethodDef != NullMethodDef.Instance 
                    && commandDef.InterceptorMethodDef != null)
                {
                    commandContext.InvocationContext.InterceptorInvocation = commandDef.InterceptorMethodDef;
                }
            }

            return next(commandContext);
        }

        private static readonly Dictionary<Type, Func<CommandContext, object>> InjectableServiceTypes = new Dictionary<Type, Func<CommandContext, object>>
        {
            [typeof(CommandContext)] = ctx => ctx,
            [typeof(IConsole)] = ctx => ctx.Console,
            [typeof(CancellationToken)] = ctx => ctx.AppConfig.CancellationToken
        };

        private static async Task<int> ResolveInstancesMiddleware(CommandContext commandContext, ExecutionDelegate next)
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
                    var ctor = classType.GetConstructors()
                        .Select(c => new {c, p= c.GetParameters()})
                        .Where(cp => cp.p.Length == 0 || cp.p.All(p => InjectableServiceTypes.ContainsKey(p.ParameterType)))
                        .OrderByDescending(cp => cp.p.Length)
                        .FirstOrDefault();

                    if (ctor == null)
                    {
                        throw new AppRunnerException($"No viable constructors found for {classType}");
                    }

                    instanceOwnedByThisMiddleware = true;
                    var parameters = ctor.p
                        .Select(p => InjectableServiceTypes[p.ParameterType](commandContext))
                        .ToArray();
                    commandContext.InvocationContext.Instance = ctor.c.Invoke(parameters);
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

        private static Task<int> InvokeCommandDefMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            Task<int> InvokeCommand(CommandContext ctx)
            {
                return ctx.InvocationContext.CommandInvocation
                    .Invoke(commandContext, ctx.InvocationContext.Instance, null)
                    .GetResultCodeAsync();
            }

            var invocations = commandContext.InvocationContext;
            return invocations.InterceptorInvocation != null
                ? invocations.InterceptorInvocation
                    .Invoke(commandContext, invocations.Instance, InvokeCommand)
                    .GetResultCodeAsync()
                : InvokeCommand(commandContext);
        }
    }
}