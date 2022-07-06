# CommandDotNet.Spectre

## 3.0.2

* support move of AppSettings.Localize to AppSettings.Localization
* ResourceProxy with memberNameAsKey, to better support resx files.

## 3.0.1

* update to dotnet 6

## 2.0.2

minor change to capture more output from IAnsiConsole.Out in tests
remove nuget package refs no longer required after move to net5.0

## 2.0.1

remove nuget package refs no longer required after move to net5.0

## 2.0.0

CommandDotNet.Spectre.Testing targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

AnsiConsoleForwardingConsole updated in response to changes in IConsole

## 1.0.1

Localize argument prompt instructions

## 1.0.0

Released support for Spectre's IAnsiConsole and using Spectre to prompt for missing arguments.

Read more about it [here](../OtherFeatures/spectre.md)
