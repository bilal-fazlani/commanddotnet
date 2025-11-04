# AppSettings Reference

`AppSettings` is the central configuration object for customizing CommandDotNet behavior. It contains nested settings objects for different aspects of the framework.

## How to Configure

AppSettings are configured when creating the AppRunner:

<!-- snippet: appsettings_constructor -->
<a id='snippet-appsettings_constructor'></a>
```cs
private static AppRunner ConstructorMethod()
{
    return new AppRunner<Program>(new AppSettings
    {
        Help = { PrintHelpOption = true }
    });
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L69-L77' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_constructor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Or using the Configure method:

<!-- snippet: appsettings_configure_method -->
<a id='snippet-appsettings_configure_method'></a>
```cs
private static AppRunner ConfigureMethod()
{
    return new AppRunner<Program>()
        .Configure(b => b.AppSettings.Help.PrintHelpOption = true);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L61-L67' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_configure_method' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Settings Categories

### Arguments Settings

`AppSettings.Arguments` - Controls argument parsing and behavior

<!-- snippet: ArgumentAppSettings -->
<a id='snippet-ArgumentAppSettings'></a>
```cs
/// <summary>
/// When Explicit, boolean options require a 'true' or 'false' value be specified.<br/>
/// When Implicit, boolean options are treated as Flags, considered false unless it's specified
/// and the next argument will be considered a new argument.
/// </summary>
public BooleanMode BooleanMode { get; set; } = BooleanMode.Implicit;

/// <summary>
/// When arguments are not decorated with <see cref="OperandAttribute"/> or <see cref="OptionAttribute"/>
/// DefaultArgumentMode is used to determine which type of argument to assign.
/// <see cref="ArgumentMode.Operand"/> is the default.
/// </summary>
public ArgumentMode DefaultArgumentMode { get; set; } = ArgumentMode.Operand;

/// <summary>
/// Character used to split the option values into substrings.
/// Setting it here will enable for all options that accept multiple values.<br/>
/// The character can be set and overridden for each option in the <see cref="OptionAttribute"/>
/// or <see cref="NamedAttribute"/>. 
/// </summary>
public char? DefaultOptionSplit { get; set; }

/// <summary>
/// Symbol used to indicate piped content should be directed to a specific argument.
/// If not specified when executing a command, piped input will be directed to
/// the final operand if it is a list.
/// </summary>
public string? DefaultPipeTargetSymbol { get; set; } = "^";

/// <summary>
/// When true, arity is not validated.
/// Arity validation will also be skipped if the application does not support
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references">NRTs</see>.
/// </summary>
public bool SkipArityValidation { get; set; }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ArgumentAppSettings.cs#L9-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-ArgumentAppSettings' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Parser Settings

`AppSettings.Parser` - Controls how arguments are parsed

<!-- snippet: ParseAppSettings -->
<a id='snippet-ParseAppSettings'></a>
```cs
/// <summary>
/// The default <see cref="ArgumentSeparatorStrategy"/>.
/// This can be overridden for a <see cref="Command"/> using the <see cref="CommandAttribute"/>
/// </summary>
public ArgumentSeparatorStrategy DefaultArgumentSeparatorStrategy { get; set; } = ArgumentSeparatorStrategy.EndOfOptions;

/// <summary>
/// When false, unexpected operands will generate a parse failure.<br/>
/// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
/// </summary>
public bool IgnoreUnexpectedOperands { get; set; }
    
public bool AllowBackslashOptionPrefix { get; set; }

public bool AllowSingleHyphenForLongNames { get; set; }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ParseAppSettings.cs#L10-L26' title='Snippet source file'>snippet source</a> | <a href='#snippet-ParseAppSettings' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Commands Settings

`AppSettings.Commands` - Controls command discovery and execution

<!-- snippet: CommandAppSettings -->
<a id='snippet-CommandAppSettings'></a>
```cs
/// <summary>When true, methods on base classes will be included as commands.</summary>
public bool InheritCommandsFromBaseClasses { get; set; }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/CommandAppSettings.cs#L9-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-CommandAppSettings' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Help Settings

`AppSettings.Help` - Customizes help generation

<!-- snippet: AppHelpSettings -->
<a id='snippet-AppHelpSettings'></a>
```cs
/// <summary>When true, the help option will be included in the help for every command</summary>
public bool PrintHelpOption { get; set; }

/// <summary>Specify whether to use Basic or Detailed help mode. Default is Detailed.</summary>
public HelpTextStyle TextStyle { get; set; } = HelpTextStyle.Detailed;

internal void SetExecutionSettings(ExecutionAppSettings executionSettings) => _executionSettings = executionSettings;

/// <summary>Specify what AppName to use in the 'Usage:' example</summary>
[Obsolete("Use AppSettings.Execution.UsageAppNameStyle")]
public UsageAppNameStyle UsageAppNameStyle
{
    get => _executionSettings?.UsageAppNameStyle ?? UsageAppNameStyle.Adaptive;
    set { if (_executionSettings != null) _executionSettings.UsageAppNameStyle = value; }
}

/// <summary>
/// The application name used in the Usage section of help documentation.<br/>
/// When specified, <see cref="UsageAppNameStyle"/> is ignored.
/// </summary>
[Obsolete("Use AppSettings.Execution.UsageAppName")]
public string? UsageAppName
{
    get => _executionSettings?.UsageAppName;
    set { if (_executionSettings != null) _executionSettings.UsageAppName = value; }
}

/// <summary>
/// When true, the usage section will expand arguments so the names of all arguments are shown.
/// </summary>
public bool ExpandArgumentsInUsage { get; set; } = true;
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Help/AppHelpSettings.cs#L13-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-AppHelpSettings' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Note
    Additional obsolete properties `UsageAppNameStyle` and `UsageAppName` have been moved to `AppSettings.Execution`

### Execution Settings

`AppSettings.Execution` - Controls command execution

<!-- snippet: ExecutionAppSettings -->
<a id='snippet-ExecutionAppSettings'></a>
```cs
/// <summary>Specify what AppName to use in usage examples, help text, and generated scripts</summary>
public UsageAppNameStyle UsageAppNameStyle { get; set; } = UsageAppNameStyle.Adaptive;

/// <summary>
/// The application name used in usage examples, help text, and generated scripts.<br/>
/// When specified, <see cref="UsageAppNameStyle"/> is ignored.
/// </summary>
public string? UsageAppName { get; set; }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/ExecutionAppSettings.cs#L11-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExecutionAppSettings' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Localization Settings

`AppSettings.Localization` - Configure localization behavior

<!-- snippet: LocalizationAppSettings -->
<a id='snippet-LocalizationAppSettings'></a>
```cs
/// <summary>When specified, this function will be used to localize user output from the framework</summary>
public Func<string,string?>? Localize { get; set; }

/// <summary>
/// By default, the keys passed to the <see cref="Localize"/> delegate
/// are the values define in the Resources class.<br/>
/// Setting this to true will use the property or method names instead of the values.<br/>
/// Example: for property - `Common_argument_lc => "argument"`<br/>
/// the default key is "argument".<br/>
/// When <see cref="UseMemberNamesAsKeys"/> is set to true, "Common_argument_lc" is the key.
/// </summary>
public bool UseMemberNamesAsKeys { get; set; }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/LocalizationAppSettings.cs#L10-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-LocalizationAppSettings' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Common Configuration Patterns

### All Arguments as Options

Make all arguments named by default:

<!-- snippet: appsettings_all_arguments_as_options -->
<a id='snippet-appsettings_all_arguments_as_options'></a>
```cs
private static AppSettings AllArgumentsAsOptions = new AppSettings
{
    Arguments = { DefaultArgumentMode = ArgumentMode.Option }
};
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L15-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_all_arguments_as_options' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Explicit Boolean Values

Require users to specify true/false for boolean options:

<!-- snippet: appsettings_explicit_boolean_values -->
<a id='snippet-appsettings_explicit_boolean_values'></a>
```cs
private static AppSettings ExplicitBooleanValues = new AppSettings
{
    Arguments = { BooleanMode = BooleanMode.Explicit }
};
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L22-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_explicit_boolean_values' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Windows-style Options

Support `/option` syntax:

<!-- snippet: appsettings_windows_style_options -->
<a id='snippet-appsettings_windows_style_options'></a>
```cs
private static AppSettings WindowsStyleOptions = new AppSettings
{
    Parser = { AllowBackslashOptionPrefix = true }
};
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L29-L34' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_windows_style_options' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### PowerShell-style Options

Support `-longname` syntax:

<!-- snippet: appsettings_powershell_style_options -->
<a id='snippet-appsettings_powershell_style_options'></a>
```cs
private static AppSettings PowerShellStyleOptions = new AppSettings
{
    Parser = { AllowSingleHyphenForLongNames = true }
};
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L36-L41' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_powershell_style_options' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Expanded Help

Show argument names in usage:

<!-- snippet: appsettings_expanded_help -->
<a id='snippet-appsettings_expanded_help'></a>
```cs
private static AppSettings ExpandedHelp = new AppSettings
{
    Help = 
    { 
        ExpandArgumentsInUsage = true,
        PrintHelpOption = true
    }
};
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L43-L52' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_expanded_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Disable Directives

Turn off directive support (e.g., `[debug]`, `[parse]`):

<!-- snippet: appsettings_disable_directives -->
<a id='snippet-appsettings_disable_directives'></a>
```cs
private static AppSettings DisableDirectives = new AppSettings
{
    DisableDirectives = true
};
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/AppSettings_Examples.cs#L54-L59' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_disable_directives' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Tips

1. **Start with defaults**: Only override settings you actually need
2. **Document your choices**: Explain non-standard configurations in code comments
3. **Test early**: Configure AppSettings early in development to avoid surprises
4. **Consider users**: Think about how settings affect the user experience

## Related

- [Arguments](../Arguments/arguments.md) - Argument configuration
- [Help](../Help/help.md) - Help customization
- [Argument Separator](../ArgumentValues/argument-separator.md) - Using the `--` separator
- [Default Middleware](default-middleware.md) - Pre-configured middleware sets
