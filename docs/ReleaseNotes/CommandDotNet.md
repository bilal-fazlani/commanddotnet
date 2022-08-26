# CommandDotNet

## 7.0.2

* add optional ResourceProxy to AppRunner<T> constructor.
* ResourceProxy with memberNameAsKey, to better support resx files.
* added AppSettings.Localization with memberNameAsKey option.
  * deprecate AppSettings.Localize for AppSettings.Localization

## 7.0.1

* update to dotnet 6
  * able to assume NRTs are enabled by default
* use `NotNullWhen(true)` on IDependencyResolver.TryResolve to inform compiler the value is null when the method returns false.
  * also for TryGetDirective middleware extension method

### Remove previously obsoleted code

* setter for CommandAttribute.Name, Operand.Name, OptionAttribute.ShortName and OptionAttribute.LongName.

## 6.0.5

Public methods from base classes can be commands. Set `AppSettings.Commands.InheritCommandsFromBaseClasses = true` to include public methods from base classes.
Methods from `System.Object` and `IDisposable` are not included.

## 6.0.4

Changed default pipe target from `$*` to `^`.  The `$` wasn't working well from bash prompts.

## 6.0.3

### Improvements

* CommandLogger no longer prints Machine and User info by default. They can be enabled via `UseCommandLogger(includeMachineAndUser: true)`
* Change type display name for doubles and floats to Decimal.  Size does not matter, just as it didn't with Number. This gives us two numeric descriptions: Number and Decimal.

### Bug fixes

* Fixed [bug](https://github.com/bilal-fazlani/commanddotnet/issues/418) where version option did not work when the program was published as a single executable file

## 6.0.2

Fix bug in arity validation where validation was skipped non-nullable value types with default values on argument models

## 6.0.1

Add message "Required command was not provided" when non-executable command was submitted without a help option like `--help`

## 6.0.0

Back with some more goodness. This fix focuses on improvements for developers and some features to support advanced use cases.

### Improvements

#### Terser definition of command and argument names

We've simplified how you can define short names and override long names. 

Until now, it worked like `[Option(ShortName="f", LongName="file")]`, `[Operand(Name = "file")]`, and `[Command(Name = "move")]`

Now available: `[Option('f', "file")]`, `[Operand("file")]`, and `[Command("move")]`

`[SubCommand]` has also been deprecated and replaced by `[Subcommand]`

The old patterns have been deprecated but is still supported for this version.

You can run [this script](https://github.com/bilal-fazlani/commanddotnet/tree/master/scripts/v6-upgrade-attrs.sh) in git bash in the folder containing your commmand definitions to update usages for these attributes. It assumes:

- The names are on the same line as the attribute
- The names are the first properties specified after the attribute

#### Arity Validation

Now that we have support for NRTs we can determine if an argument is required or not based on whether it's an NRT, Nullable<T> or optional parameter. 

If it is required and no value is provided via configs or the cli, the command will fail. Since this is a breaking change in behavior, it can be disabled by setting `AppSettings.Arguments.SkipArityValidation = true`.

#### Localizable Descriptions, Usage & ExtendedHelpText

If you've set the AppSettings.Localize func, the HelpTextProvider will pass Usage, ExtendedHelpText, and Description (for commands and arguments) through the func.

#### IConsole moved

IConsole has been moved to the root CommandDotNet namespace. While tis is a breaking change, most files where you're using IConsole probably already reference CommandDotNet which means you're likely to have unused usings than build failures.

#### Error handling

The CommandDotNet exceptions have been consolidated into two types 

- Dev errors (InvalidConfigurationException)
- User errors (ValueParsingException)

These errors are intercepted and displayed to the user before the registered error handler is called. This ensures your error handler can focus on exceptions from your app. We've also ensured stack traces will not be shown for these exceptions

### Bug Fixes

* fixed bug where streaming into an IEnumerable<T> where T is not a string would crash

### Bug Fixes

* fixed bug where streaming into an IEnumerable<T> where T is not a string would crash

### New Features

#### IEnvironment

Added IEnvironment for testability. Covers most of System.Environment

IEnvironment can be injected into command methods.  eg `Move(IConsole console, IEnvironment env, string file)`

#### Piped Input targeting

Piped input can now target any argument. By default, piped input will be unioned into inputs for any type of IEnumerable<T> Operand if it exists.

Using `$*` as a value for an argument will union the piped input to that argument instead.  i.e. `find ... | move --files $* ~/tmp/` will move the files into the users tmp directory.

`$*` can be overridden using AppSettings.Arguments.DefaultPipeTargetSymbol. It can also be overridden using the `[pipeto:...]` directive to avoid conflicts in scripts.

#### Splitting multi-value options

To provide multiple values for an option, you've had to repeat the option name each time, eg. `--name jack --name jill`

Now you can define a separator character to use to split a string. `[Optiom(Split=",")]` and then `--name=jack,jill`

A global default can be set using AppSettings.Arguments.DefaultOptionSplit

The user can also override by using the `[split:-]` directive for `--name=jack-jill`.

#### Subcommand rename

One of the non-obvious but powerful features of CommandDotNet is that a command can be reused as a subcommand for several commands. It's likely the desired to reuse the same name for consistency, but there could be exceptions.

The subcommand attribute now contains a `RenameAs` property. When used, the command will use that name instead of the name defined in the `CommandAttribute` or derived from the class name.

#### EnumerableCancellationExtensions

Added to new extensions for enumerable to make it easier to exit commands when Ctrl+C is pressed. 

- `items.UntilCancelled(cancellationToken)`
- `items.ThrowIfCancelled(cancellationToken)`

## 5.0.1

remove nuget package refs no longer required after move to net5.0

## 5.0.0

### Highlights

#### NRT
CommandDotNet now supports [Nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/nullable-reference-types) (NRT) when calculating Arity.  Arityfor.Minimum will be 0 for NRTs.

#### target net5.0
CommandDotNet targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.
We are eager to take advantage of [Source Generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) though so we will likely also target net6.0 in early 2022.

#### Windows and Powershell conventions
Added support for Windows and Powershell conventions when using options

* backslash `/help` and `/h` for windows conventions 
* single hypen `-help` for long names for powershell conventions

These are not enabled by default. Enable by setting `AppSettings.Parser.AllowBackslashOptionPrefix` and/or `AppSettings.Parser.AllowSingleHyphenForLongNames` = `true`.

#### Support negative numbers as arguments
Previously, negative numbers were only support when proceeded by `:` or `=`, eg `--amount=-15` which meant they could only be used with options. CommandDotNet now supports `--amount -15` or `-15` as an operand.

#### IConsole
Overhaul of IConsole, now covering most the System.Console members. Converted In, Out and Error to TextReader and TextWriter to match System.Console.
This includes updates to the TestConsole and AnsiTestConsole for the Spectre integration.

Overhaul of IConsole, now covering most the System.Console members. Converted In, Out and Error to TextReader and TextWriter to match System.Console.
This includes updates to the TestConsole and AnsiTestConsole for the Spectre integration.

### Breaking Changes in behavior

#### NRT support

The behavior for calculating Arity and prompting on missing arguments changes as these arguments were not previously considered nullable and so the Arity has changed to expect a minimum of 0 instead of 1

Using the UseArgumentPrompter will now work as expected when using NRTs. They will no longer prompt the user.

#### ArgumentArity.AllowsNone() change

This has been a confusing method. It currently evaluates as `arity.Maximum == 0` but it intuitively it makes more sense as `arity.Minimum == 0`.

ArgumentArity.AllowsNone() has been changed to `arity.Minimum == 0`

ArgumentArity.RequiresNone() has been added for `arity.Maximum == 0`.


### Breaking Changes in API

#### For application developers

##### Obsoleted cleanup

* removed `DefaultMethodAttribute`. Use `DefaultCommandAttribute` instead.
* removed `appRunner.UsePrompting(...)` extension method. Use `.UseIPrompter` and `.UseArgumentPrompter` instead.
* removed `BooleanMode.Unknown`. Use either `Implicit`, `Explicit`, or `BooleanMode?` instead.

##### Localizable Resources
Several updates to localizeable Resources. Changed a few member names for clarity and anded a several new members.

##### AppSettings

* Added AppSettings.Parser
* Moved IgnoreUexpectedOperands to AppSettings.Parser.IgnoreUnexpectedOperands


#### For middleware developers

##### IArgument updates

* added BooleanMode properties to help determine the Arity for an argument.
* BooleanMode will only be set for boolean arguments

##### IConsole updates

* added numerous members to bring closer to parity with System.Console
* removed StandardStreamReader and StandardStreamWriter
* added ForwardingTextWriter to support integrations where text forwards to another console utility, such as Spectre's AnsiConsole
* SystemConsole and TestConsole can be inherited for simpler adaptation resiliant to breakimg changes in IConsole if new members are added

##### DefaultSources.AppSetting.GetKeysFromConvention

Now takes AppSettings to support `/` and `-` option prefixes.

##### TokenType.Option removed

To support negative numbers and prefixing options with `/` and `-`, we needed more context from the command definition so determining if a token is an option or value has been moved to the command parser. Token transformation  no longer distinguishes options vs values. 

* TokenType.Option and TokenType.Value have merged into TokenType.Argument
* OptionTokenType and Token.OptionTokenType have been removed
* Clubbed options are no longer split during token transformation
* Option assignments are no longer separated during token transformation
* Tokenizer.TryTokenizeOption has been removed

##### IInvocation updates

added IsInterceptor property to distinguish between command and interceptor invocations

##### Obsoleted cleanup

* removed `ArgumentArity.Default(Type, ...)`. Use `ArgumentArity.Default(IArgument)` instead. This is a reversal of previous direction, but the addition of IArgument.BooleanMode makes this reliable.
* removed `FindOption(string alias)`. Use `Find<Option>(string alias)` instead.
* removed `command.GetIgnoreUnexpectedOperands(AppSettings)`. Use `command.IgnoreUnexpectedOperands` as it now defaults from AppSettings.
* removed `command.GetArgumentSeparatorStrategy(AppSettings)`. Use `command.ArgumentSeparatorStrategy` as it now defaults from AppSettings.
* removed `Option.ShowInHelp`. Use `Option.Hidden` instead. There were cases where this had meaning outside of generating help.

## 4.3.0

* add `[Named]` and `[Positional]` attributes which can be used in place of `[Option]` and `[Operand]` respectively.
* fixed bug in argument prompting where optional operands were being prompted
* include release notes link in nuget package

## 4.2.3

* Prompting
    * remove `exludePrompting` from `UseDefaultMiddleware` configuration extension
    * obsoleted `UsePrompting`, breaking it into two methods: `UsePrompter` and `UseArgumentPrompter`
    * argumentFilter must now include arity check and if argument has value.  See example in [../ArgumentValues/prompting.md]
* Added IArgument extensions to simplify creating argument filters: HasValueFromInput, HasValueFromDefault, & HasValueFromInputOrDefault
* obsoleted `DefaultMethodAttribute` in favor of `DefaultCommandAttribute` to be consistent with naming in the rest of the framework
* When converting from string, types with string ctor or static Parse methods can now contain additional optional parameters as long as there is a single required parameter of type string.
* For middleware developers, CommandContext.Services will now find fallback to AppConfig.Services.

## 4.2.2

* Finished localizing strings that would appear for user in normal settings. What remains are configuration exceptions that should be discovered during the development phase. There are no plans to localize these, but we accept PRs if it's critical for you.
* resx and json translations files can be found in the [localization_files](https://github.com/bilal-fazlani/commanddotnet/tree/master/localization_files) folder. 

## 4.2.1

* Added the [Culture directive](../Localization/culture-directive.md) for testing
* Removed Resources.Error_Type_is_not_supported_as_argument(type.FullName)). It's should only be seen by developers during testing.

## 4.2.0

* Added *beta* support for [Localization](../Localization/overview.md).
* Added the [Time directive](../Diagnostics/time-directive.md)
* Fixed bug where NonSerializableWrapper used to capture date in Exception.Data was not correctly outputting it's contents when printing exceptions.

## 4.1.13

Fix [bug](https://github.com/bilal-fazlani/commanddotnet/issues/346) where calling a command using option syntax threw an exception.

Now it will suggest the correct syntax to the user.

4.1.9-4.1.12 were skipped while converting our build system over to GitHub Actions

## 4.1.8

Expose DefaultSources.GetValueFunc for host apps to reuse logic for alternate configuration sources.

See the new [.Net Core Config](../ArgumentValues/default-values-from-config.md#.net-core-config) section for an example.

## 4.1.7

AppInfo.ToString will print out all properties making it easier to gather info for diagnostics.

Expose object.ToStringFromPublicProperties to make it easier to output other classes for diagnostics.

## 4.1.6

Add AppInfo.SetResolver to enable overriding for tests to get a consistent AppName. See [Deterministic AppName for tests](../TestTools/Tools/deterministic-appinfo.md) for details.

## 4.1.5

Fixes [bug #331](https://github.com/bilal-fazlani/commanddotnet/issues/331) on .Net Framework when capturing CommandContext in Exception.Data.  In .Net Framework, items added to Data must be   Thanks to `@arendvw` for reporting this.

## 4.1.4

Fixes [bug #326](https://github.com/bilal-fazlani/commanddotnet/issues/326) where password from default values could be displayed in plain text in help. Thanks to `@giuliov` for reporting this. 

## 4.1.3

Fixes a bug where the TypoSuggest middleware threw an exception when an argument value was an empty string. Thanks to `@taori` for reporting this.

## 4.1.2

Fixes a bug where values were not assigned to options added via the builder.

## 4.1.1

Fixes a bug where a negative number can treated as a short name. eg. `add -5 10` would have resulted in -5 being parsed as an option. 

Add restriction that short names can only be letters. If an arg can be parsed as a negative number, it will not be confused with a short name.

## 4.1.0

### unrequested help returns 1 instead of 0

When help is shown because a given command is not executable, an error code of 1 is returned to indicate a command was not successfully run.

### new types supported for arguments

In addition to types with a TypeConverter or Ctor with a single string parameter. any type with a public static Parse method with a single string parameter are now eligible as types for arguments.

### API

* AppRunnerException will not be passed to the error handler. Previously they were always printed to the console.

### Obsoleted

* `ArgumentArity.Default(IArgument)` for the other ArgumentArity.Default method. It was inadequate in some cases and could lead to bugs.
* `BooleanMode.Unknown` in favor of `BooleanMode?`.
* `AppInfo.GetAppinfo(CommandContext)` in favor of `AppInfo.GetAppInfo()`. The former exists to support CommandDotNet tests.
* `Command.GetIgnoreUnexpectedOperands(AppSettings)` & `Command.GetArgumentSeparatorStrategy(AppSettings)` in favor of the properties on the command which reflect AppSettings defaults.


## 4.0.2

Make FlattenedArgumentModels via the IInvocation. 
This makes it easier to access all argument models for an invocation, including those defined as properties of other models.
It also prevents additional reflection calls to walk a hierarchy. The work was already done and now it's shared.

## 4.0.1

Add PropertyInfo or ParameterInfo to IArgument.Services. This makes it possible for middleware to access them from a command and to determine if an argument is from a parameter or property.

## 4.0.0

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### Default behavior changes

Version 4 is removing obsolete members and changing default behaviors made possible since the v3 was introduced.

* default `AppSettings.Help.ExpandArgumentsInUsage` to true.
    * arguments are expanded in the usage section of help.
        * old: `add [options] [arguments]`
        * new: `add [options] <x> <y>`
* default `AppSettings.Parser.DefaultArgumentSeparatorStrategy` to `EndOfOptions`. See [Argument Separator](../ArgumentValues/argument-separator.md) for details.
    * Help will append ` [[--] <arg>...]` to the usage example when `DefaultArgumentSeparatorStrategy=PassThru`
* make `AppSettings.LongNameAlwaysDefaultsToSymbolName` the only behavior and remove the setting. `LongName` can be removed with `[Option(LongName=null)]`.
    * Look for places in your apps where `[Option(ShortName="a")]` with setting a LongName. If you don't want a LongName then add `LongName=null` otherwise the long name will default from the parameter or property name.
* make `AppSettings.GuaranteeOperandOrderInArgumentModels` the only behavior and remove the setting.
    * see this [Argument Models section](../Arguments/argument-models/#guaranteeing-the-order-of-arguments) for details
* enable [CommandLogger](../Diagnostics/command-logger.md) in `.UseDefaultMiddleware()` as `cmdlog` directive. This can be useful for diagnostics.
* Command.FindOption will throw an exception if the alias is for an operand or subcommand instead of an option. Previously it would return null.

#### Added

* The type `MiddlewareSteps` declares most of the framework defined middleware to make it easier to inject your custom middleware in the desired order. 

#### Changed

* When registering middleware, the OrderInStage parameter has been changed from `int` to `short`.

#### Moved

* `CommandDotNet.Directives.Parse.ParseReporter` - moved to `CommandDotNet.Diagnostics.Parse.ParseReporter`
* `CommandDotNet.Directives.Debugger` - moved to `CommandDotNet.Diagnostics.Debugger`
* `AppConfig.CancellationToken` - moved to `CommandDotNet.CancellationToken`. This enables running nested commands within an interactive session.  See [Ctrl+C and CancellationToken](../OtherFeatures/cancellation.md#interactive-sessions) for more details.

#### Removed or Replaced

* `AppSettings`
    * `MethodArgumentMode` - replaced by `DefaultArgumentMode`
    * `ThrowOnUnexpectedArgument` - replaced by `IgnoreUnexpectedArguments`
    * `AllowArgumentSeparator` - was never used for functionality, only to show `--` in help.
    * `HelpTextStyle` - replaced by `Help.TextStyle`
    * `Help`
        * `GlobalTool` - replaced by `UsageAppName`
* `VersionInfo` - replaced by `AppInfo`
* `ApplicationMetadataAttribute` - replaced by `CommandAttribute`
* `ArgumentAttribute` - replaced by `OperandAttribute`
* `ArgumentMode.Parameter` - replaced by `Operand`
* `InjectPropertyAttribute` - v3 made ctor injection possible and that should be used.
* `Command`, `Option`, `Operand` ctor taking with `Command parent` parameter. `Parent` is now assigned by adding to a command.
* `OptionAttribute`
    * `Inherited` - replaced by `AssignToExecutableSubcommands`
* `Option`
    * `DefaultValue` - use `Option.Default`
    * `Inherited` - replaced by `AssignToExecutableSubcommands`
* `Operand`
    * `DefaultValue` - use `Option.Default`
* `IArgument`
    * `DefaultValue` - use `Option.Default`
* `appRunner.UseDefaultsFromConfig` extension method that returns string, in favor of method with same name returning `ArgumentDefault`
* `TokenCollection` public ctor - Use `Tokenizer.Tokenize` extension method to generate tokens and `TokenCollection.Transform` to transform them. Ensures source tokens are correctly mapped.
* `AnsiConsole` - no longer supported. Use a package like ColorConsole or Pastel.
* `MiddlewareSteps.Help.Stage` & `MiddlewareSteps.Help.Order` - replaced by nested `MiddlewareSteps.Help` classes
* `ServicesExtensions.GetOrAdd<T>` - use `ContextDataExtensions.GetOrAdd<T>`

## 3.6.5

#### Typo suggestions for argument values

Typo Suggestions middleware now also makes suggestions for [for argument values](../Help/typo-suggestions.md#suggestions-for-argument-values).

#### Bug: Subcommands should not be parsed after first operand

Subcommands were still being searched by the parser after the first operand was provided. This bug has been fixed.

## 3.6.4

Reimplement UseErrorHandler(CommandContext, Exception). See updated [documentation](../Diagnostics/exceptions.md).

Fix bug where parser did not fail when an option value was skipped when immediately followed by another option.

## 3.6.3

Remove .UseErrorHandler config method until the bug is fixed. Updated documentation with the approach.

## 3.6.2

* An informative error message will output when a [response file](../ArgumentValues/response-files.md) is not found.
* TokenizerPipeline, on error, 
    * no longer throws a TokenTransformationException
    * now prints error messages to IConsole.Error, logs the exception and return ExitCodes.Error (1)
    * this is best practice for console middleware.
* added `ExitCodes` class with `Success`, `Error` & `ValidationError`. These can be used in the middleware pipeline.
* added MiddlewareSteps.ErrorHandler step to be used to register error handlers
* added CommandLogger.HasLoggedFor and made CommandLogger.Log safe to use before ParseResult is populated. This will be useful for logging on error. See [Exceptions > Printing config information](../Diagnostics/exceptions.md#printing-config-information) for details

## 3.6.1

### Remove extraneous NewLine

A few components could leave an extra NewLine hanging around. This has been removed for cleaner test exception handling.  Affects CommandLogger, FluentValidation, ParseDirective and when AppRunner prints an exception before exit.

## 3.6.0

### Feature

#### Argument Separator following [Posix Guideline](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html#tag_12_02) 10

> The first -- argument that is not an option-argument should be accepted as a delimiter indicating the end of options. Any following arguments should be treated as operands, even if they begin with the '-' character.

Before this release, all arguments following `--` were captured to `CommandContext.ParseResult.SeparatedArguments` and not made available for parsing. That prevented use of operands with values beginning with `-` or `--` or enclosed in square brackets when directives were not disabled.

So `calculator.exe add -1 -2` was not possible.  With this release, `calculator.exe add -- -1 -2` is possible.  

All arguments following `--` are still captured to `CommandContext.ParseResult.SeparatedArguments`, but now may include values for operands of the current command.  Compare w/ `CommandContext.ParseResult.RemainingOperands` to see if any were mapped into the command.

See [Argument Separator](../ArgumentValues/argument-separator.md) for more help.

As part of this update, `CommandContext.ParseResult.SeparatedArguments` && `CommandContext.ParseResult.RemainingOperands` were changed from `IReadOnlyCollection<Token>` to `IReadOnlyCollection<string>`. 

#### CommandLogger.Log

Make CommandLogger a public class so commands, interceptors and middleware can run it directly.

This makes the last pattern in the [Command Logger](../Diagnostics/command-logger.md) help possible, using an interceptor option to trigger the log.

### API

#### CommandContext.ShowHelpOnExit

You can now trigger help to be display after validation checks have failed. See [help docs](../Help/help.md#printing-help) for details.

#### CommandContext.PrintHelp()

`PrintHelp()` extension method enables printing help from anywhere there's a CommandContext

#### exception.GetCommandContext()

Most every exception that escapes the appRunner will have a CommandContext in the Data property.

Use the `GetCommandContext()` extension method to get it and then PrintHelp or ParseReporter.Report or ...

#### CommandAttribute parse hints

The followingw were added to the CommandAttribute to override AppSettings for the given command.

* `IgnoreUnexpectedArguments` to override `AppSettings.IgnoreUnexpectedArguments`
* `ArgumentSeparatorStrategy` to override `AppSettings.Parser.DefaultArgumentSeparatorStrategy`

#### Console Write___ extension methods

Write an object to Console.Out and Console.Error.  The object will be converted to string.

* `Write(this IStandardStreamWriter writer, object value)`
* `WriteLine(this IStandardStreamWriter writer, object value)`

Use `console.Write` and `console.WriteLine` to write to console.Out.

* `Write(this IConsole console, object value)`
* `WriteLine(this IConsole console, object value)`

## 3.5.1

Fix bug: when enabled w/ default configuration, Command Logger no longer outputs unless [cmdlog] is specified

## 3.5.0

### Feature

#### Command Logger

A middleware that will output...

* parse directive results
* optionally some system info: os version, .net version, machine name, ...
* optionally AppConfig
* can configure to include additional info with access to CommandContext

... and then run the command. Output can be forwarded to console or logs or ...

See [Command Logger help](../Diagnostics/command-logger.md) for more details

#### AppSettings.LongNameAlwaysDefaultsToSymbolName 

This setting will default to true in the next major version.

When setting `[Option(ShortName="a")]`, the LongName would no longer default to the parameter or property name and so you'd have to explicitely set the LongName if you wanted both. We want to change that behavior so that if you only want a short name, you'd need to `[Option(ShortName="a", LongName=null)]`.
AppSettings.LongNameAlwaysDefaultsToSymbolName allows us to introduce the behavior in a non-breaking manner until the next release.

[#183](https://github.com/bilal-fazlani/commanddotnet/issues/183)

## 3.4.0

### Feature

#### Enhanced Parse directive

The [parse directive](../Diagnostics/parse-directive.md) has been updated to show 

* argument values
* default values w/ sources 
* input values with sources, including response files if enabled.

Password values are replaced with ***** when the `Password` type is used.

### API

#### TokenCollection public ctor deprecated

Use Tokenizer.Tokenize extension method to generate tokens and TokenCollection.Transform to transform them

## 3.3.0

### Feature

#### Fix bug: support multiple default value config sources

There was a bug when using multiple config sources for default values, i.e. EnvVar and AppSettings.
The last one set would override previous ones. Resolved in this release.

### API

#### add ext method IArgument.IsObscured

Returns true if the type is `Password`

## 3.2.0

### API

#### Disable LogProvider by default

* LogProvider.IsDisabled is set to true in AppRunner static ctor.

#### [Guarantee Operand order in IArgumentModel](../Arguments/argument-models.md/#guaranteeing-the-order-of-arguments)

The order of operands defined in IArgumentModel classes were never deterministic because .Net does not guarantee the order properties are reflected.

[CallerLineNumber] was used in the OperantAttribute ctor to ensure the order is always based on the order properties are defined in the class.

!!! Warning
    The issue wasn't correctly resolved until 4.0.0

## 3.1.0

### Feature

#### Typo Suggestions enhancements

Support mis-ordered keywords: nameuser -> username

Trim results & improve accuracy

#### Support self-contained executable 

[AppInfo](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Builders/AppInfo.cs) interrogates the MainModule and EntryAssembly to determine if the app is run from `dotnet`, as a self-contained executable or a standard executable. This is used for help and to determine the correct app version.

[#197](https://github.com/bilal-fazlani/commanddotnet/issues/197), 
[#179](https://github.com/bilal-fazlani/commanddotnet/issues/179)

### API

#### AppInfo replaces VersionInfo

VersionInfo became a subset of features of AppInfo

#### IArgument.Default and ArgumentDefault

ArgumentDefault was introduced to track the source of a default value. We can determine if it came from source or an external source and the key used.
This is used in later versions for the Parse directive and CommandLogger feature.



## 3.0.2

### Feature

#### .UseDefaultMiddleware()

- added [Typo Suggestions](#typo-suggestions)

#### Typo Suggestions

[Typo Suggestions](../Help/typo-suggestions.md) middleware to suggest commands or options when a provided one wasn't found.

#### AppSettings.ExpandArgumentsInUsage

Expand arguments in the usage section so the names and order of all arguments are shown.

See [help docs](../Help/help.md#expandargumentsinusage) for more details.

[#186](https://github.com/bilal-fazlani/commanddotnet/issues/186), 

### API

#### Setting parent commands
Option & Operand & Command should now be created without a parent command. Parent will be assigned when added to a command.



## 3.0.1

### Feature

#### [%UsageAppName%](../Help/help.md#usageappname-tempate) 
To show usage examples in a command description, extended help or overridden usage section, use %UsageAppName%. This text will be replaced usage app name from one of the options above. See [the help docs](../Help/help.md#usageappname-tempate) for more.

### API

#### improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```
## 3.0.0
[Version 3 Change Summary](whats-new-in-v3.md)
