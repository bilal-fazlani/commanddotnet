# CommandDotNet

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
* `ArgumentSeparatorStrategy` to override `AppSettings.DefaultArgumentSeparatorStrategy`

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

#### [Guarantee Operand order in IArgumentModel](../Arguments/argument-models.md/#guaranteeing-the-order-of-operands)

The order of operands defined in IArgumentModel classes were never deterministic because .Net does not guarantee the order properties are reflected.

[CallerLineNumber] was used in the OperantAttribute ctor to ensure the order is always based on the order properties are defined in the class.

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
