using System;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;

namespace CommandDotNet.TestTools
{
    public static class CommandContextExtensions
    {
        /// <summary>
        /// Returns the <see cref="InvocationStep"/> for the <see cref="ParseResult.TargetCommand"/>
        /// </summary>
        public static InvocationStep GetCommandInvocationStep(this CommandContext ctx)
        {
            return ctx.InvocationPipeline?.TargetCommand;
        }

        /// <summary>
        /// Returns the <see cref="TCommandClass"/> for the <see cref="ParseResult.TargetCommand"/>
        /// </summary>
        public static TCommandClass GetCommandInstance<TCommandClass>(this CommandContext ctx) where TCommandClass : class
        {
            return (TCommandClass)ctx.GetCommandInvocationStep()?.Instance;
        }

        /// <summary>
        /// Returns the <see cref="IInvocation"/> for the <see cref="ParseResult.TargetCommand"/>
        /// </summary>
        public static IInvocation GetCommandInvocation(this CommandContext ctx)
        {
            return ctx.GetCommandInvocationStep()?.Invocation;
        }

        /// <summary>
        /// Returns the <see cref="TrackingInvocation"/> for the <see cref="ParseResult.TargetCommand"/>
        /// </summary>
        public static TrackingInvocation GetCommandTrackingInvocation(this CommandContext ctx)
        {
            return ctx.GetCommandInvocation<TrackingInvocation>();
        }

        /// <summary>
        /// Returns the <see cref="IInvocation"/> as <see cref="TInvocation"/> for the <see cref="ParseResult.TargetCommand"/>
        /// </summary>
        public static TInvocation GetCommandInvocation<TInvocation>(this CommandContext ctx) where TInvocation: IInvocation
        {
            return GetAndCastInvocation<TInvocation>(ctx, GetCommandInvocation);
        }

        /// <summary>
        /// Returns the <see cref="InvocationStep"/> for the interceptor defined in the <see cref="TInterceptorClass"/><br/>
        /// Will be null when the interceptor was not in the hierarchy of the executed command.
        /// </summary>
        public static InvocationStep GetInterceptorInvocationStep<TInterceptorClass>(this CommandContext ctx) where TInterceptorClass : class
        {
            return ctx.InvocationPipeline?.All?
                .FirstOrDefault(i => i.Invocation.MethodInfo.DeclaringType == typeof(TInterceptorClass));
        }

        /// <summary>
        /// Returns the instance of the <see cref="TInterceptorClass"/><br/>
        /// Will be null when the interceptor was not in the hierarchy of the executed command.
        /// </summary>
        public static TInterceptorClass GetInterceptorInstance<TInterceptorClass>(this CommandContext ctx) where TInterceptorClass : class
        {
            return (TInterceptorClass)ctx.GetInterceptorInvocationStep<TInterceptorClass>()?.Instance;
        }

        /// <summary>
        /// Returns the <see cref="IInvocation"/> for the interceptor defined in the <see cref="TInterceptorClass"/><br/>
        /// Will be null when the interceptor was not in the hierarchy of the executed command.
        /// </summary>
        public static IInvocation GetInterceptorInvocation<TInterceptorClass>(this CommandContext ctx) where TInterceptorClass: class
        {
            return ctx.GetInterceptorInvocationStep<TInterceptorClass>()?.Invocation;
        }

        /// <summary>
        /// Returns the <see cref="TrackingInvocation"/> for the interceptor defined in the <see cref="TInterceptorClass"/><br/>
        /// Will be null when the interceptor was not in the hierarchy of the executed command.
        /// </summary>
        public static TrackingInvocation GetInterceptorTrackingInvocation<TInterceptorClass>(this CommandContext ctx)
            where TInterceptorClass : class =>
            ctx.GetInterceptorInvocation<TInterceptorClass, TrackingInvocation>();

        /// <summary>
        /// Returns the <see cref="IInvocation"/> as <see cref="TInvocation"/> for the interceptor defined in the <see cref="TInterceptorClass"/><br/>
        /// Will be null when the interceptor was not in the hierarchy of the executed command.
        /// </summary>
        public static TInvocation GetInterceptorInvocation<TInterceptorClass, TInvocation>(this CommandContext ctx) 
            where TInterceptorClass : class where TInvocation : IInvocation
        {
            return GetAndCastInvocation<TInvocation>(ctx, GetInterceptorInvocation<TInterceptorClass>);
        }

        private static TInvocation GetAndCastInvocation<TInvocation>(this CommandContext ctx, Func<CommandContext, IInvocation> callback) where TInvocation : IInvocation
        {
            try
            {
                return (TInvocation)callback(ctx);
            }
            catch (InvalidCastException e)
            {
                if (typeof(TInvocation) == typeof(TrackingInvocation))
                {
                    var isTrackingConfigured = ctx.AppConfig.MiddlewarePipeline.Any(p =>
                        p.Method.Name == nameof(AppRunnerTestConfigExtensions.TrackingInvocationMiddleware));

                    if (isTrackingConfigured)
                    {
                        throw new InvalidCastException(
                            $"{e.Message}. " +
                            $"The {nameof(AppRunner)} is configured with " +
                            $"'appRunner.{nameof(AppRunnerTestConfigExtensions.InjectTrackingInvocations)}()'. " +
                            "Is another middleware wrapping or replacing the assignment?", e);
                    }

                    throw new InvalidCastException(
                        $"{e.Message}. " +
                        $"Configure the {nameof(AppRunner)} with " +
                        $"'appRunner.{nameof(AppRunnerTestConfigExtensions.InjectTrackingInvocations)}()'", e);
                }

                throw;
            }
        }
    }
}