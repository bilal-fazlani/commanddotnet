# CommandDotNet.Spectre.Testing

## 3.0.1

* update to dotnet 6

## 2.0.2

option to trim ending whitespace when getting test console output

Fix bug where Error output was not being reported by AnsiTestConsole. 
This has a change in behavior from the CommandDotNet TestConsole. All Error output will appear after all Out output.
The Spectre TestConsole captures only Output and not through a path we can intercept. 
This prevents us from determining the order in which Error and Out were entered in relation to each other.

## 2.0.1

remove nuget package refs no longer required after move to net5.0

## 2.0.0

CommandDotNet.Spectre.Testing targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

AnsiTestConsole updated in response to changes in IConsole and TestConsole

## 1.0.0

Test support for IAnsiConsole

Read more about it [here](../OtherFeatures/spectre.md)
