# Middleware Pipeline

The architecture uses a middleware pipeline, similar to [ASP.NET Core](https://thomaslevesque.com/2018/03/27/understanding-the-asp-net-core-middleware-pipeline/) and [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/How-To#middleware-pipeline).

The middleware pipeline is a type of [chain of responsibilty pattern](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern).  
Middleware are components that register a delegate as a step in the pipeline.  The delegate is called by the previous delegate and in turn calls the next delegate.  With this pattern, a middleware can perform work before and after the call to the next delegate or skip the next delegate if appropriate.  Take the Help middleware for example. When `-h`, `--help` or `-?` been specified, the help middleware prints help for the target command and does not call the next middleware.

As the middleware executes, it enhances the `CommandContext`.  The `CommandContext` contains the context relevate to the current command. For example... the tokenizer middleware populates the `CommandContext.Tokens` property.  The parser middleware populates the `CommandContext.ParseResults` property.  The `CommandContext` is part of the delegate signature and is therefore available for all middleware.

### Why we use the middleware pattern
* Ease of adding new features without touching existing code. Observing [Single Resposibility](https://en.wikipedia.org/wiki/Single_responsibility_principle) &  [Open-Closed](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle) principals.
* Ease of extending the framework using external packages. Supports community enhancements while reducing the bloat of the framework code base.
* Feature tests can exclude middleware that's not relevant to the tests.

![Middleware](../diagrams/MiddlewarePipeline.png){:style="float: right;"}

### Middleware Stages
The core functionality of the framework and optional features have been implemented using middleware. 
One of the classic challenges of using the middleware pattern is understanding the order middleware components should be registered. To address this challenge, the pipeline has been split into eight distinct changes, as seen in the diagram below, four core stages (in green) and four extensibility stages (in yellow). The four core stages contain the core functionality of the framework. After each core stage is complete, the framework guarantees specific parts of the `CommandContext` are populated.

The other four stages are where other middleware will typically be registered, based on `CommandContext` properties the middleware requires to operate. For example, `PromptForMissingOperands` is registered in the `PostParseInputPreBindValues` stage because it depends on the `ParseResults.ArgumentValues` property being populated in the `ParseInput` stage. `FluentValidation` is registerd in the `PostBindValuesPreInvoke` stage because it relies on the `IInvocation.ParameterValues` being populated in the `BindValues` stage.  Read more in the intellisense of `MiddlewareStages` and the properties of the `CommandContext` classes.

Note: `DisplayVersion` is the only optional middleware in a core stage. Any middleware can be registered in a core stage if needed. In the case of `DisplayVersion`, the middleware must run before `DisplayHelp` otherwise help would trigger for the root command when it is not executable.


### Invocation Pipeline
The invocation pipeline is assembled during the `ParseInput` stage, captured in `CommandContext.InvocationPipeline` property and run during the `InvokeInvocationPipeline` stage. The pipeline consists of [interceptor methods](interceptors.md) and the target command method.