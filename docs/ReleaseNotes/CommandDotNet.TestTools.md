# CommandDotNet.TestTools

## 2.0.0 (upcoming)

This release focuses on usability, readability and discoverability.

The tooling was initially designed for CommandDotNet with an aspiration for general use. The tooling did work for other projects but required those projects to write shims to make the tooling more useful.

The primary test extensions: `RunInMem` and `Verify` are now easier to use, provide a more consistent experience and no longer duplicated output in some scenarios.

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

### AppRunnerResult.Console___ properties moved to TestConsole

* AppRunnerResult.ConsoleOutAndError -> AppRunnerResult.Console.AllText()
* AppRunnerResult.ConsoleOut -> AppRunnerResult.Console.OutText()
* AppRunnerResult.ConsoleError -> AppRunnerResult.Console.ErrorText()

### AppRunnerResult.ConsoleOut -> Console.AllText()

Renamed AppRunnerResult.ConsoleOutAndError to ConsoleAll

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