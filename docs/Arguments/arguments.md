# Arguments

Arguments are defined as parameters of a method or properties of a class (see [Argument Models](argument-models.md))

As discussed in [Terminology](../argument-terminology.md), Option and Operand are the two concrete types of IArgument

* Option: named argument
* Operand: positional argument

By default, arguments are operands. Change the default by assigning `#!c# AppSettings.DefaultArgumentMode = ArgumentMode.Option`

Use the `[Operand]` (or `[Positional]`) and `[Option]` (or `[Named]`) attributes to explicity denote which argument type and to configure the arguments.

The Positional and Named attributes are provided for those who prefer that terminology for defining commands. The terms operand and option are still used for these arguments elsewhere in the framework.

!!!Tip
    See [Option vs Operand](option-or-operand.md) for recommendations on when to use one vs the other.

## Operand Attribute

If preferred, you can use `[Named]` instead of `[Operand]` with the same properties

### Properties

* __Name__: Used in help documentation only. Defaults to the parameter or property name.
* __Description__: Used in help documentation.

## Option Attribute

If preferred, you can use `[Positional]` instead of `[Option]` with the same properties

### Names
The option long name is defaulted from the property or parameter name that defines them. 
The case can be changed using the [name-casing](../OtherFeatures/name-casing.md) middleware.

The long name can be overridden using the attribut constructor `[Option("new-name")]`

A short name can be added using the attribute constructor `[Option('a')]`

Both can be supplied using `[Option('n', "new-name")]`

To force only a short name for the option, set the long name to null: `[Option('n', null)]`

### Properties
The OptionAttribute has the following properties:

* __Description__: Used in help documentation.
* __DescriptionLines__: Used in help documentation, and honors the host systems newline characters.
* __BooleanMode__: When the option is a `bool`, this determines if the presence of the option 
  indicates `true` (_Implicit_) or if the user must specify `true` or `false` (_Explicit_). 
    * The default is _Implicit_ and can be changed with `#!c# AppSettings.Arguments.BooleanMode = BooleanMode.Explicit`
    * _Implicit_ boolean options are also called __Flags__
* __Split__: use with list options to specify the character used to split multiple values
    * If not set, AppSettings.Arguments.DefaultOptionSplit is used if set
    * in the example `--names alex,amy,joe`, the comma is the split character
* __AssignToExecutableSubcommands__: only valid when used in [Interceptor](../Extensibility/interceptors.md) methods.

## Example

<!-- snippet: arguments_attributes -->
<a id='snippet-arguments_attributes'></a>
```c#
public void LaunchRocket(
    [Operand("planet", Description = "Name of the planet you wish the rocket to go")]
    string planetName,
    [Option('t', "turbo", Description = "Do you want to go fast?")]
    bool turbo,
    [Option('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)]
    bool abort)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Attributes.cs#L12-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_attributes' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

or

<!-- snippet: arguments_attributes_alt -->
<a id='snippet-arguments_attributes_alt'></a>
```c#
public void LaunchRocket(
        [Positional("planet", Description = "Name of the planet you wish the rocket to go")]
        string planetName,
        [Named('t', "turbo", Description = "Do you want to go fast?")]
        bool turbo,
        [Named('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)]
        bool abort)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Attributes.cs#L26-L34' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_attributes_alt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

and help looks like:

<!-- snippet: arguments_attributes_help -->
<a id='snippet-arguments_attributes_help'></a>
```bash
$ mission-control.exe LaunchRocket --help
Usage: mission-control.exe LaunchRocket [options] <planet>

Arguments:

  planet  <TEXT>
  Name of the planet you wish the rocket to go

Options:

  -t | --turbo
  Do you want to go fast?

  -a | --abort  <BOOLEAN>
  Abort the launch before takeoff
  Allowed values: true, false
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_attributes_help.bash#L1-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_attributes_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

and called any of these ways:

```bash
mission-control.exe LaunchRocket -t -a true mars
mission-control.exe LaunchRocket --turbo -a true mars
mission-control.exe LaunchRocket mars -t -a true
mission-control.exe LaunchRocket mars --turbo -a true
```

Options are not positional so they can appear in any order within the command.

| Parameter name | Longname | Shortname | Generated template
| --- | --- | --- | --- |
| turbo |  |  | `--turbo` |
| turbo | turbo |  | `--turbo` |
| turbo |  | t | `-t` |
| turbo | turbo | t | `-t | --turbo` |
| t |  |  | `-t` |

!!! Tip
    To configure an option to have only a short name, set the long name to null `[Option('a', null)]`

## Flags

Flags are boolean options with a default value of false. The presence of the flag indicates true. This simplifies the user experience by allowing them to specifiy `-b` instead of `-b true`.  This also enables clubbing.

Define them as Options with BooleanMode = BooleanMode.Implicit. Implicit is the default defined for `AppSettings.Arguments.DefaultBooleanMode` or `OptionAttribute.BooleanMode`.

## Flag Clubbing

Clubbing (aka bundling) is when several flags are specified together using their short names. 

For example: `-abc` is the same as `-a -b -c`

Clubbing is only available for flags by short name.

## Option assignments

When assigning option values, the following are the same

* `--time tomorrow`
* `--time=tomorrow`
* `--time:tomorrow`

When assigning multiple values, by default each value will need to be proceeded by the option name

`--days Monday --days Tuesday`

To let the user use a delimiter such as `--days Monday,Tuesday`, you must specify the split character to use.
The split character can be set globally for use by all multi-value options using `AppSettings.Arguments.DefaultOptionSplit`
The split character can also be set per option using `[Option(Split=',')]`. The value set for an option will override the default.

## Support for Windows and Powershell option prefixes

By default CommandDotNet follows POSIX conventions and uses `-` to indicate an option short name and `--` to indicate an option long name.

While this convention has been adopted by many programs that run in Windows, the legacy convention is to use only `\` for both short and long names.

The Powershell convention is to use `-` for both short and long names.

With version 5, CommandDotNet supports both conventions. 

The existing POSIX conventions are still the default and what appear in help. 

Support for the Windows and Powershell conventions is intented to provide backwards compatibility for existing applications being ported to CommandDotNet where scripts and tooling expects the other conventions.

How to enable and use with Windows

<!-- snippet: AppSettings_for_windows -->
<a id='snippet-appsettings_for_windows'></a>
```c#
new AppSettings { Parser = { AllowBackslashOptionPrefix = true } };
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Attributes.cs#L61-L63' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_for_windows' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: arguments_attributes_windows_exe -->
<a id='snippet-arguments_attributes_windows_exe'></a>
```bash
$ mission-control.exe LaunchRocket /turbo /a true mars
planet=mars turbo=True abort=True
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_attributes_windows_exe.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_attributes_windows_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

How to enable and use with Powershell

<!-- snippet: AppSettings_for_powershell -->
<a id='snippet-appsettings_for_powershell'></a>
```c#
new AppSettings { Parser = { AllowSingleHyphenForLongNames = true } };
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Attributes.cs#L76-L78' title='Snippet source file'>snippet source</a> | <a href='#snippet-appsettings_for_powershell' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: arguments_attributes_powershell_exe -->
<a id='snippet-arguments_attributes_powershell_exe'></a>
```bash
$ mission-control.exe LaunchRocket -turbo -a true mars
planet=mars turbo=True abort=True
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_attributes_powershell_exe.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_attributes_powershell_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
