# CommandDotNet.TestTools

## 6.0.2

* support ResourceProxy classes with memberNameAsKey, to better support resx files.

## 6.0.1

* update to dotnet 6
*  use `NotNullWhen(true)` on TestDependencyResolver.TryResolve to inform compiler the value is null when the method returns false.
* update ResourcesDef.GenerateProxyClass to specify nullable parameters

## 5.0.1

Added [TestEnvironment](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestEnvironment.cs) and `.UseTestEnv(...)` registration method for tests.

Obsoleted ListAnswer and TextAnswer with by adding the constructors to Answer, improving discoverability.

## 5.0.0

Changing default behavior of TestConsole to trim whitespace from the end of the output. This removes the trailing newlines, making the test ssertions cleaner and more what you'd expect.

This change can be reverted using TestConfig,SkipTrimEndOfConsoleOutputs or passing in a TestConsole(trimEnd: false)

### Intercept System.Console

Sometimes you have the luxury of using IConsole to capture test output. We've added the `appRunner.InterceptSystemConsoleWrites()`. 
Intercepts Console.Out and Console.Error. Text is still written to those writers, but also to IConsole.

This is not currently suitable for tests run in parallel, but we accept PRs if you'd like to see this functionality in CommandDotNet.

## 4.0.1

remove nuget package refs no longer required after move to net5.0

## 4.0.0

### target net5.0
CommandDotNet.TestTools targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

### Breaking Changes
TrackingInvocation implements new IInvocation.IsInterceptor property

TestConsole updated to support new IConsole members. In, Out, and Error now implement TextReader and TextWriter.
SystemConsole and TestConsole can be inherited for simpler adaptation resiliant to breakimg changes in IConsole if new members are added.

### Removed Obsoleted 

* Removed TestConcolse constructor containing mock paramters. Use the Mock methods instead.

## 3.1.2

Extracted `ITestConsole` interface from `TestConsole` to support the [Spectre extensions](../OtherFeatures/spectre.md)

## 3.1.1

Added to  `ResourcesDef`

* `IsMissingMembersFrom` so tests can identify when a proxy does not override members of the base.
* `GetMembersWithDefaults` to help generate resource files as shown in [ResourceGenerators](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/UnitTests/Localization/ResourceGenerators.cs)

## 3.1.0

Tooling support for [Localization](../Localization/overview.md). 

Added [ResourcesDef](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/ResourcesDef.cs) to simplify loading members of a [Resources](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Resources.cs) file and generate a corresponding [ResourcesProxy](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ResourcesProxy.cs).

Use `ResourcesDef` to generate a ResourcesProxy for Resources you've defined in your middleware packages 

## 3.0.2

TestConfig.AppInfoOverride to enable getting a consistent AppName. See [Deterministic AppName for tests](../TestTools/Tools/deterministic-appinfo.md) for details.

## 3.0.1

Making FlattenedArgumentModels available via TrackingInvocation

## 3.0.0

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### Obsoletes

Remove obsoleted TestCaptures class

## 2.1.1

### Getting invocations in tests

Adding CommandContext extensions `GetCommandInvocation` and `GetInterceptorInvocation<TInterceptorClass>` that give access to invocation of the command and interceptor methods.

With these extensions, tests for middleware can verify the values passed to the methods instead of using TestCaptures.

### Goodbye TestCaptures

TestCaptures and associated methods have been marked obsolete. They'll be removed in the next major version. This was added before v3 as a way to cpature the parameter values passed into methods.  With v3 complete, it's easy to access the parameter values via `CommandContext.InvocationPipeline`. Scenarios can now `AssertContext` which removes the need for TestCaptures. This also removes the need for the custom ShouldBeEquivalentTo.

## 2.1.0

Remove FluentValidator requirement. Implemented simple ShouldBeEquivalentTo that handles IEnumerables, primitives and deep recursion of public properties of objects. This is only used with TestCaptures.

Obsoleted the Output___ members of AppRunnerResult. They'll be remove in the next release.

## 2.0.1

More assert options for BDD Scenario `Then = { AssertOutput..., AssertContext... }`. See [BDD Verify > Then](../TestTools/Harness/bdd.md#then) for details.

## 2.0.0

This release focuses on usability, readability and discoverability.

The tooling was initially designed for CommandDotNet with an aspiration for general use. The tooling did work for other projects but required those projects to write shims to make the tooling more useful.

The primary test extensions: `RunInMem` and `Verify` are now easier to use, provide a more consistent experience and no longer duplicate output in some scenarios.

### PromptResponder

* Extract IAnswer and split answers into separate types: TextAnswer, ListAnswer, Answer, FailAnswer
* replace FailOnPromptResponder with FailAnswer

### TestConfig

TestConfig provides settings primarily to determine what is logged during a test run, on success and on error.

See [TestConfig docs](../TestTools/Harness/test-config.md) for more details.

### Scenario.Then Output vs Result

Clean up naming confusion in Scenarios: Ouputs vs Result. 

TestOutputs represented objects (usually parameters) that were captured in a command method. 
They help test middleware and framework components. The `Then.Outputs` used a similar name but it 
was more easily interpretted as `Console.Out` which was actually found in `Then.Result`.

Renaming TestOutputs to TestCaptures made this simpler.

To update, use find/replace

* TestOutputs > TestCaptures
* Outputs = > Captures =
* Result = > Output =
* ResultsContainsTexts > OutputContainsTexts
* ResultsNotContainsTexts > OutputNotContainsTexts

### Scenerio.Given moved to Scenario.When

The properties in Given were all user inputs that would occur after the arguments are provided so it makes more sense to include them in When.

Given is the context of the fully configured AppRunner.

This better aligns with the intent of the Given-When-Then style.

### Scenario.WhenArgs & WhenArgsArray moved to Scenario.When

`Scenario.WhenArgs = ...` -> `Scenario.When = {Args = ...}`

### AppRunnerResult.Console___ properties moved to TestConsole

* AppRunnerResult.ConsoleOutAndError -> AppRunnerResult.Console.AllText()
* AppRunnerResult.ConsoleOut -> AppRunnerResult.Console.OutText()
* AppRunnerResult.ConsoleError -> AppRunnerResult.Console.ErrorText()

### AppRunnerResult.ConsoleOut -> Console.AllText()

Renamed AppRunnerResult.ConsoleOutAndError to ConsoleAll

### AppRunnerResult Output assertions

No longer trim trailing whitespace and replace line endings. 

This was originally necessary to support the HelpTextProvider and inconsistencies with NewLine's used.  Those have all been fixed so we no longer need to do this.

### Scenario.Given.AppSettings

Removed ScenarioGiven.AppSettings. It did not work outside of old CommandDotNet test infrastructure. Apologies for any confusion it may have caused.

### VerifyScenario -> Verify

renamed VerifyScenario extension method to Verify. It reads better as `appRunner.Verify(new Scenario{...})`

### ILogger -> Action<string>
replaced ILogger with Action<string>. ILogger never ended up provided more value and was a needless abstraction.

## 1.1.1

RunInMem will print the stacktrace for of a caught exception before returning the error response, otherwise the stacktrace is lost.

## 1.1.0

Test prompting no longer prints an extraneous `$"IConsole.ReadLine > {input}"` as it's not consistent with console output. This is logged instead.

## 1.0.2

Ensure log provider is always configured for appRunner RunInMem and VerifyScenario extensions

## 1.0.1

make TestToolsLogProvider publicly available

VerifyScenario now captures exceptions that escape AppRunner.Run, putting them into the result to verify as console output.  
This better mimics the behavior of the shell which will return an error code of 1 and print the exception

VerifyScenario will also logging more context on error, including most of AppConfig

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```
