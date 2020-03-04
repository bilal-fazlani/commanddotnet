# Help

Help documentation is automatically generated for the application.

## Default Template

The default template is as follows

```bash
{description}

Usage: {application} [command] [arguments] [options]

Arguments:

  {name}  (Multiple) <TYPE> [default] 
  {description}
  {allowed-values}

Options:

  -{short} | --{long} (Multiple) <TYPE> [default] 
  {description}
  {allowed-values}

Options also available for subcommands:

  -{short} | --{long} (Multiple) <TYPE> [default] 
  {description}
  {allowed-values}

Commands:

  {name} {description}

{extended-help-text}

Use "{application} [command] --help" for more information about a command.
```

Segments that do not apply for a command are not included.  

For example, the last line are only included when a command has subcommands
and "Options also available for subcommands:" is only included when options 
defined in parent commands can be provided in subcommands.

## Modify behavior via settings

There are a few settings to alter the default behavior, accessed via `AppSettings.Help`

```c#
new AppSettings
{
    Help
    {
        PrintHelpOption = true,
        TextStyle = HelpTextStyle.Basic,
        UsageAppNameStyle = UsageAppNameStyle.Executable,
        UsageAppName = "GlobalToolName"
    }
}
```

### PrintHelpOption
`PrintHelpOption` will include the help option in the list of options for every command.

### TextStyle
Default is `HelpTextStyle.Detailed`. `HelpTextStyle.Basic` changes the argument and option template to just `{name} {description}`

### UsageAppNameStyle

This determines what is used for {application} in `Usage: {application}`. 

When `Executable`, the name will be `Usage: {filename}`.

When `DotNet`, the name will be `Usage: dotnet {filename}`

When `Adaptive`, if the file name ends with ".exe", then uses `Executable` else `DotNet`.

`Adaptive` & `Executable` will also detect when the app is a [self-contained executable](https://docs.microsoft.com/en-us/dotnet/core/deploying/#produce-an-executable) and use the executable filename.

### UsageAppName

When specified, this value will be used and `UsageAppNameStyle` will be ignored.

## Custom Help Provider

The template is driven by the [HelpTextProvider](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet/Help/HelpTextProvider.cs) class.

There are two options for overriding the behavior. 

The first and simplest option is to inherit from `HelpTextProvider` and override the method for the section to modify.

The second option is to implement a new [IHelpProvider](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet/Help/IHelpProvider.cs).

Configure the provider with: `appRunner.Configure(b => b.CustomHelpProvider = new MyHelpProvider());`