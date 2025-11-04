# Source Generator Testing Strategy

## Overview

Ensure generated code produces **identical runtime behavior** to reflection-based code across all CommandDotNet features.

## 1. 🏗️ Multi-Layer Testing Approach

### Layer 1: Generator Unit Tests
**Verify the generator produces correct code**

```csharp
// Test the generator itself (requires Microsoft.CodeAnalysis.Testing)
[Fact]
public void Generator_Creates_Builder_For_SimpleCommand()
{
    var source = @"
        public class Calculator {
            public void Add(int x, int y) { }
        }";
    
    var generated = GeneratorTestHelper.Run<CommandClassGenerator>(source);
    
    generated.Should().Contain("Calculator__CommandClassBuilder");
    generated.Should().Contain("BuildLocalCommands");
}
```

### Layer 2: Integration Tests  
**Verify generated code works at runtime**

```csharp
[Fact]
public void GeneratedBuilder_Produces_Same_CommandDef_As_Reflection()
{
    var reflectionDef = new ClassCommandDef(typeof(Calculator), context);
    var generatedDef = Calculator__CommandClassBuilder.CreateCommandDef(context);
    
    // Compare command definitions
    AssertCommandDefsEqual(reflectionDef, generatedDef);
}
```

### Layer 3: End-to-End Tests
**Verify commands execute correctly**

```csharp
[Fact]
public void GeneratedCommand_Executes_Correctly()
{
    var result = new AppRunner<Calculator>()
        .UseGeneratedBuilders() // Force using generated code
        .RunInMem("add 2 3");
    
    result.ExitCode.Should().Be(0);
    result.Output.Should().Contain("5");
}
```

## 2. 📋 Feature Coverage Matrix

Create test commands that cover ALL features:

| Feature | Test Class | Status |
|---------|------------|--------|
| **Basic Commands** |
| Simple method commands | `SimpleCommands` | ⬜ |
| Multiple commands | `MultiCommands` | ⬜ |
| Async commands | `AsyncCommands` | ⬜ |
| **Parameters** |
| Method parameters | `MethodParamCommands` | ⬜ |
| Argument models | `ArgModelCommands` | ⬜ |
| Nested argument models | `NestedArgModels` | ⬜ |
| Default values | `DefaultValueCommands` | ⬜ |
| Optional parameters | `OptionalCommands` | ⬜ |
| **Arguments** |
| Options | `OptionCommands` | ⬜ |
| Operands | `OperandCommands` | ⬜ |
| Mixed options/operands | `MixedCommands` | ⬜ |
| Collections | `CollectionCommands` | ⬜ |
| Split arguments | `SplitCommands` | ⬜ |
| **Advanced** |
| Interceptors | `InterceptorCommands` | ⬜ |
| Default commands | `DefaultCommands` | ⬜ |
| Property subcommands | `PropertySubcommands` | ⬜ |
| Nested class subcommands | `NestedClassSubcommands` | ⬜ |
| **Attributes** |
| [Command] | `CommandAttributeTests` | ⬜ |
| [Subcommand] | `SubcommandAttributeTests` | ⬜ |
| [Option] / [Operand] | `ArgumentAttributeTests` | ⬜ |
| **Integrations** |
| Validators (FluentValidation) | `ValidatorCommands` | ⬜ |
| IoC resolution | `IoCCommands` | ⬜ |
| Middleware | `MiddlewareCommands` | ⬜ |

## 3. 🔬 Comparison Testing Pattern

**Gold Standard: Reflection vs Generated must be identical**

```csharp
public abstract class SourceGenComparisonTestBase
{
    protected void AssertBehaviorMatches<TCommand>(string args)
    {
        // Run with reflection
        var reflectionResult = new AppRunner<TCommand>()
            .UseReflection() // Force reflection path
            .RunInMem(args);
        
        // Run with generated code
        var generatedResult = new AppRunner<TCommand>()
            .UseGeneratedBuilders() // Force generated path
            .RunInMem(args);
        
        // Compare everything
        generatedResult.ExitCode.Should().Be(reflectionResult.ExitCode);
        generatedResult.Output.Should().Be(reflectionResult.Output);
        generatedResult.ErrorOutput.Should().Be(reflectionResult.ErrorOutput);
    }
}

public class InterceptorTests : SourceGenComparisonTestBase
{
    [Fact]
    public void Interceptor_Executes_In_Same_Order()
    {
        AssertBehaviorMatches<InterceptorCommand>("do-something");
    }
}
```

## 4. 🎭 Test Command Library

Create a **comprehensive test command library** that exercises every feature:

```
CommandDotNet.Tests/
  SourceGen/
    TestCommands/
      Basic/
        SimpleCommand.cs
        MultiCommand.cs
        AsyncCommand.cs
      Parameters/
        MethodParamCommand.cs
        ArgModelCommand.cs
        NestedArgModelCommand.cs
      Advanced/
        InterceptorCommand.cs
        DefaultCommand.cs
        SubcommandCommand.cs
      Integration/
        ValidatorCommand.cs
        IoCCommand.cs
    ComparisonTests/
      BasicCommandTests.cs
      ParameterTests.cs
      AdvancedTests.cs
      IntegrationTests.cs
```

Example test command:

```csharp
// TestCommands/Advanced/InterceptorCommand.cs
public class InterceptorCommand
{
    private readonly List<string> _log = new();
    
    public Task<int> Intercept(InterceptorExecutionDelegate next, IConsole console)
    {
        console.WriteLine("Before");
        var result = next();
        console.WriteLine("After");
        return result;
    }
    
    public void Do()
    {
        Console.WriteLine("Doing");
    }
}

// ComparisonTests/AdvancedTests.cs
public class InterceptorTests : SourceGenComparisonTestBase
{
    [Fact]
    public void Interceptor_Wraps_Command_Execution()
    {
        AssertBehaviorMatches<InterceptorCommand>("do");
        // Should output: Before\nDoing\nAfter
    }
}
```

## 5. 🏃 Snapshot Testing

Use snapshot testing to detect ANY behavioral changes:

```csharp
[Fact]
public void Generated_Output_Matches_Snapshot()
{
    var result = RunWithGenerated<ComplexCommand>("complex --opt val arg1 arg2");
    
    // Compare against known-good snapshot
    Approvals.Verify(result.ToString());
}
```

## 6. 🔍 Property-Based Testing

Test with random inputs to find edge cases:

```csharp
[Property]
public void Generated_Handles_Any_Valid_Arguments(string[] args)
{
    var reflectionResult = RunWithReflection<TestCommand>(args);
    var generatedResult = RunWithGenerated<TestCommand>(args);
    
    generatedResult.Should().BeEquivalentTo(reflectionResult);
}
```

## 7. 📊 Performance Testing

Verify the performance improvement:

```csharp
[Fact]
public void Generated_Is_Faster_Than_Reflection()
{
    var reflectionTime = BenchmarkRunner.Run<ReflectionBenchmark>();
    var generatedTime = BenchmarkRunner.Run<GeneratedBenchmark>();
    
    generatedTime.Should().BeLessThan(reflectionTime);
}
```

## 8. 🐛 Diagnostic Testing

Test error cases work correctly:

```csharp
[Fact]
public void Generated_Produces_Same_Error_Messages()
{
    var reflectionResult = RunWithReflection<BadCommand>("invalid args");
    var generatedResult = RunWithGenerated<BadCommand>("invalid args");
    
    generatedResult.ErrorOutput.Should().Be(reflectionResult.ErrorOutput);
}
```

## 9. 🔄 Regression Testing

**Every existing test should pass with generated code:**

```csharp
// Add to existing test base class
public abstract class TestBase
{
    protected AppRunner<T> CreateRunner<T>()
    {
        return new AppRunner<T>()
            .UseGeneratedBuildersIfAvailable(); // Use generated when present
    }
}
```

Run entire test suite with environment variable:
```bash
# Test with generated code
COMMANDDOTNET_USE_GENERATED=true dotnet test

# Test with reflection (control)
COMMANDDOTNET_USE_GENERATED=false dotnet test
```

## 10. 📝 Test Checklist Template

For each new feature:

```markdown
## Feature: [Name]

- [ ] Generator produces correct code
- [ ] Generated code compiles
- [ ] Runtime behavior matches reflection
- [ ] Error messages are identical
- [ ] Help text is identical
- [ ] All attributes respected
- [ ] Edge cases handled
- [ ] Performance improvement measured
- [ ] Documentation updated
```

## Implementation Priority

1. **Phase 1: Core** (do this first)
   - Simple commands
   - Method parameters
   - Basic options/operands
   - Comparison test harness

2. **Phase 2: Advanced**
   - Interceptors
   - Default commands
   - Argument models
   - Nested models

3. **Phase 3: Complex**
   - Subcommands (property & nested)
   - Validators
   - IoC integration

4. **Phase 4: Polish**
   - Performance benchmarks
   - Property-based tests
   - Full regression suite

## Success Criteria

✅ **Generator is production-ready when:**

1. All feature matrix tests pass ✓
2. Existing test suite passes with generated code ✓
3. Performance improvement ≥ 50% ✓
4. Zero behavior differences from reflection ✓
5. Works with all middleware/extensions ✓
6. Documentation complete ✓

## Recommended Tools

- **Generator testing**: `Microsoft.CodeAnalysis.Testing`
- **Snapshot testing**: `ApprovalTests.Net` or `Verify`
- **Property testing**: `FsCheck` or `Hedgehog`
- **Benchmarking**: `BenchmarkDotNet`
- **Coverage**: Verify 100% of features tested

## Example Test Structure

```csharp
namespace CommandDotNet.Tests.SourceGen.Comparison
{
    public class BasicCommandTests : ComparisonTestBase
    {
        [Theory]
        [InlineData("add 1 2", "3")]
        [InlineData("add -5 5", "0")]
        public void SimpleCommand_Works(string args, string expected)
        {
            AssertOutputMatches<Calculator>(args, expected);
        }
    }
    
    public abstract class ComparisonTestBase
    {
        protected void AssertOutputMatches<T>(string args, string expected)
        {
            var refOut = RunWithReflection<T>(args);
            var genOut = RunWithGenerated<T>(args);
            
            refOut.Should().Be(genOut);
            genOut.Should().Contain(expected);
        }
    }
}
```
