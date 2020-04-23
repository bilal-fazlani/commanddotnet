using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using FluentAssertions.Collections;

namespace CommandDotNet.Tests.Utils
{
    public static class AssertionExtensions
    {
        /// <summary>Should be same order</summary>
        public static AndConstraint<StringCollectionAssertions> BeEquivalentSequenceTo(
            this StringCollectionAssertions assertions,
            params string[] expectedValues)
        {
            return assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));
        }

        /// <summary>Should be same order</summary>
        public static AndConstraint<GenericCollectionAssertions<object>> BeEquivalentSequenceTo(
            this GenericCollectionAssertions<object> assertions,
            params object[] expectedValues)
        {
            return assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));
        }

        /// <summary>Being explicit brings clarity in tests</summary>
        public static void ParamValuesShouldBeEmpty(this CommandContext ctx) => ctx.ParamValuesShouldBe();

        public static void ParamValuesShouldBe(this CommandContext ctx, params object[] values)
        {
            var invocation = ctx.GetCommandInvocation();
            InvocationParamValuesShouldBe(ctx, values, invocation);
        }

        /// <summary>Being explicit brings clarity in tests</summary>
        public static void ParamValuesShouldBeEmpty<TInterceptorClass>(this CommandContext ctx) 
            where TInterceptorClass : class => 
            ctx.ParamValuesShouldBe<TInterceptorClass>();

        public static void ParamValuesShouldBe<TInterceptorClass>(this CommandContext ctx, params object[] values)
            where TInterceptorClass : class
        {
            var invocation = ctx.GetInterceptorInvocation<TInterceptorClass>();
            InvocationParamValuesShouldBe(ctx, values, invocation);
        }

        private static void InvocationParamValuesShouldBe(CommandContext ctx, object[] values, IInvocation invocation)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            invocation.Parameters
                .Where(p => IsArgumentParameter(p, ctx))
                .Select(p => invocation.ParameterValues?[p.Position])
                .Should().BeEquivalentSequenceTo(values);
        }

        private static bool IsArgumentParameter(ParameterInfo info, CommandContext ctx) =>
            !IsNotArgumentParameter(info, ctx);

        private static bool IsNotArgumentParameter(ParameterInfo info, CommandContext ctx)
        {
            return info.ParameterType == typeof(InterceptorExecutionDelegate)
                   || info.ParameterType == typeof(ExecutionDelegate)
                   || ctx.AppConfig.ParameterResolversByType.ContainsKey(info.ParameterType);
        }
    }
}