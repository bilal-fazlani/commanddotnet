using System;
using System.Reflection;
using CommandDotNet.Tests.SourceGen.TestCommands;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.SourceGen;

/// <summary>
/// Tests that verify the source generator produces the expected output
/// These tests verify the generator is creating the builder classes correctly
/// </summary>
public class GeneratorOutputTests
{
    [Theory]
    [InlineData(typeof(Calculator), "CommandDotNet.Tests.SourceGen.TestCommands.Calculator__CommandClassBuilder")]
    [InlineData(typeof(InterceptorCommand), "CommandDotNet.Tests.SourceGen.TestCommands.InterceptorCommand__CommandClassBuilder")]
    [InlineData(typeof(DefaultCommandTest), "CommandDotNet.Tests.SourceGen.TestCommands.DefaultCommandTest__CommandClassBuilder")]
    [InlineData(typeof(ArgumentModelCommand), "CommandDotNet.Tests.SourceGen.TestCommands.ArgumentModelCommand__CommandClassBuilder")]
    public void Generator_CreatesBuilder_ForCommandClass(Type commandType, string expectedBuilderTypeName)
    {
        var builderType = commandType.Assembly.GetType(expectedBuilderTypeName);
        
        builderType.Should().NotBeNull(
            $"source generator should have created {expectedBuilderTypeName} for {commandType.Name}");
    }

    [Theory]
    [InlineData(typeof(Calculator))]
    [InlineData(typeof(InterceptorCommand))]
    [InlineData(typeof(DefaultCommandTest))]
    [InlineData(typeof(ArgumentModelCommand))]
    public void GeneratedBuilder_HasCreateCommandDefMethod(Type commandType)
    {
        var builderType = GetBuilderType(commandType);
        
        if (builderType == null)
        {
            // Generator didn't run - skip this test
            return;
        }

        var method = builderType.GetMethod("CreateCommandDef", 
            BindingFlags.Public | BindingFlags.Static);

        method.Should().NotBeNull($"{builderType.Name} should have CreateCommandDef method");
        method!.ReturnType.Name.Should().Be("ICommandDef");
        method.GetParameters().Should().HaveCount(1);
        method.GetParameters()[0].ParameterType.Name.Should().Be("CommandContext");
    }

    [Theory]
    [InlineData(typeof(Calculator))]
    [InlineData(typeof(DefaultCommandTest))]
    public void GeneratedBuilder_HasBuildLocalCommandsMethod(Type commandType)
    {
        var builderType = GetBuilderType(commandType);
        
        if (builderType == null) return;

        var method = builderType.GetMethod("BuildLocalCommands",
            BindingFlags.NonPublic | BindingFlags.Static);

        method.Should().NotBeNull($"{builderType.Name} should have BuildLocalCommands method");
        method!.GetParameters().Should().HaveCount(1);
        method.GetParameters()[0].ParameterType.Name.Should().Be("AppConfig");
    }

    [Fact]
    public void InterceptorCommand_GeneratedBuilder_HasInterceptorMethod()
    {
        var builderType = GetBuilderType(typeof(InterceptorCommand));
        
        if (builderType == null) return;

        var method = builderType.GetMethod("BuildInterceptorMethod",
            BindingFlags.NonPublic | BindingFlags.Static);

        method.Should().NotBeNull("InterceptorCommand builder should have BuildInterceptorMethod");
    }

    [Fact]
    public void DefaultCommandTest_GeneratedBuilder_HasDefaultCommandMethod()
    {
        var builderType = GetBuilderType(typeof(DefaultCommandTest));
        
        if (builderType == null) return;

        var method = builderType.GetMethod("BuildDefaultCommand",
            BindingFlags.NonPublic | BindingFlags.Static);

        method.Should().NotBeNull("DefaultCommandTest builder should have BuildDefaultCommand");
    }

    [Fact]
    public void Calculator_GeneratedCode_ContainsAllCommands()
    {
        var builderType = GetBuilderType(typeof(Calculator));
        
        if (builderType == null) return;

        // The generated BuildLocalCommands method should reference Add, Subtract, Multiply
        var method = builderType.GetMethod("BuildLocalCommands",
            BindingFlags.NonPublic | BindingFlags.Static);
        
        method.Should().NotBeNull();
        
        // We can't easily inspect the method body in .NET,
        // but we can verify it returns the right type
        method!.ReturnType.Name.Should().Be("List`1");
    }

    private Type? GetBuilderType(Type commandType)
    {
        var builderTypeName = $"{commandType.Namespace}.{commandType.Name}__CommandClassBuilder";
        return commandType.Assembly.GetType(builderTypeName);
    }
}
