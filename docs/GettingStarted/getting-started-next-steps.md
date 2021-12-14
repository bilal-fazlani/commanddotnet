# Opt-In to additional features

In the `Program.Main`, we configured the app with the basic feature set.

<!-- snippet: getting-started-other-features -->
<a id='snippet-getting-started-other-features'></a>
```c#
new AppRunner<Program>()
    .UseDefaultMiddleware();
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_OtherFeatures.cs#L10-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-other-features' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

`UseDefaultMiddleware` to take advantage of many more additional features, such as
[debug](../Diagnostics/debug-directive.md) directive, 
[parse](../Diagnostics/parse-directive) directive,
[time](../Diagnostics/time-directive) directive,
[ctrl+c support](../OtherFeatures/cancellation.md),
[response files](../ArgumentValues/response-files.md),
and [typo suggestions](../Help/typo-suggestions.md)

_see [Default Middleware](../OtherFeatures/default-middleware.md) for more details and options for using default middleware._

## Next Steps

You get the gist of this library now. This may be all you need to start your app.  If not, we've only touched on a small number of our features. Check out our documentation for more...

* [Commands](../Commands/commands.md) defining commands, subcommands and arguments.

* [Arguments](../Arguments/arguments.md) defining arguments.

* [Argument Values](../ArgumentValues/argument-separator.md) providing values to arguments.

* [Help](../Help/help.md) options to modify help and other help features. 
 
* [Diagnostics](../Diagnostics/app-version.md) a rich set of tools to simplify troubleshooting

* [Other Features](../OtherFeatures/default-middleware.md) additional features available.

* [Extensibility](../Extensibility/directives.md) if the framework is missing a feature you need, you can likely add it yourself. For questions, ping us on our [Discord channel](https://discord.gg/QFxKSeG) or create a [GitHub Issue](https://github.com/bilal-fazlani/commanddotnet/issues)

* [Test Tools](../TestTools/overview.md) a test package to test console output with this framework. These tools enable you to provide end-to-end testing with the same experience as the console as well as testing middleware and other extensibility components. This package is used to test all of the CommandDotNet features.
