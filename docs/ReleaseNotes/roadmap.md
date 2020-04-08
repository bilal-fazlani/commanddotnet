# Roadmap

## CommandDotNet v4 Breaking Changes

This is where you can track plans for the next major version of CommandDotNet that
will result in breaking changes to behavior or the API.

### Features
* default `AppSettings.ExpandArgumentsInUsage` to true.
* default `AppSettings.LongNameAlwaysDefaultsToSymbolName` to true.
* default `AppSettings.DefaultArgumentSeparatorStrategy` to `EndOfOptions`.

### API

#### moved
  * `CommandDotNet.Directives.Parse.ParseReporter` - moved to `CommandDotNet.Diagnostics.Parse.ParseReporter`
  * `CommandDotNet.Directives.Debugger` - moved to `CommandDotNet.Diagnostics.Debugger`

#### removed or replaced

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
