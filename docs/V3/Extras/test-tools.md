# Test Tools

CommandDotNet features are tested using a BDD framework built specificly for this framework.

The framework is ideal for testing middleware components and end-to-end testing to verify configuration. It includes tools for 

* `PromptResponder`
  * Respond to and verify prompt requests, based on OnReadKey. Works with `IPrompter` middleware.
  * See [tests](https://github.com/bilal-fazlani/commanddotnet/tree/beta-v3/master/CommandDotNet.Tests/FeatureTests/Prompting) for examples.
* `TestConsole`
  * capture output
  * provide piped input
  * handle ReadLine and ReadToEnd
* `TempFiles` 
  * Creates temp files, give content and receive a file path.
  * Removes created files on dispose
* `TestDependencyResolver` a simple implementation of `IDependencyResolver`
  * usage: `appRunner.UseDependencyResolver(new TestDependencyResolver {dbSvc, httpSvc})`
* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.

More details about Scenario testing can be found in our readme files [here](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet.Tests.README.md)
and [here](https://github.com/bilal-fazlani/commanddotnet/blob/beta-v3/master/CommandDotNet.Tests/FeatureTests/README.md)


nuget package: [CommandDotNet.TestTools](https://www.nuget.org/packages/CommandDotNet.TestTools)