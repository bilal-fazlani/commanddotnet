using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using FluentAssertions;
using FluentAssertions.Collections;
using JetBrains.Annotations;

namespace CommandDotNet.Tests.Utils;

[PublicAPI]
public static class AssertionExtensions
{
    /// <summary>Should be same order</summary>
    public static AndConstraint<StringCollectionAssertions<IEnumerable<string>>> BeEquivalentSequenceTo(
        this StringCollectionAssertions assertions,
        params string[] expectedValues) =>
        assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));

    /// <summary>Should be same order</summary>
    public static AndConstraint<GenericCollectionAssertions<object?>> BeEquivalentSequenceTo(
        this GenericCollectionAssertions<object?> assertions,
        params object?[] expectedValues) =>
        assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));

    /// <summary>Being explicit brings clarity in tests</summary>
    public static void ParamValuesShouldBeEmpty(this CommandContext ctx) => ctx.ParamValuesShouldBe();

    public static void ParamValuesShouldBe(this CommandContext ctx, params object?[] values)
    {
        var invocation = ctx.GetCommandInvocationInfo();
        InvocationParamValuesShouldBe(values, invocation);
    }

    /// <summary>Being explicit brings clarity in tests</summary>
    public static void ParamValuesShouldBeEmpty<TInterceptorClass>(this CommandContext ctx) 
        where TInterceptorClass : class => 
        ctx.ParamValuesShouldBe<TInterceptorClass>();

    public static void ParamValuesShouldBe<TInterceptorClass>(this CommandContext ctx, params object?[] values)
        where TInterceptorClass : class
    {
        var invocation = ctx.GetInterceptorInvocationInfo<TInterceptorClass>();
        InvocationParamValuesShouldBe(values, invocation);
    }

    private static void InvocationParamValuesShouldBe(object?[] values, InvocationInfo invocation)
    {
        ArgumentNullException.ThrowIfNull(values);
        invocation.ArgumentParameterValues.Should().BeEquivalentSequenceTo(values);
    }
}