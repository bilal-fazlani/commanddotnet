# Middleware Pipeline

The architecture uses a middleware pipeline, similar to [ASP.NET Core](https://thomaslevesque.com/2018/03/27/understanding-the-asp-net-core-middleware-pipeline/) and [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/How-To#middleware-pipeline).

The middleware pipeline is a type of [chain of responsibilty pattern](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern).  
Middleware are components that register a delegate as a step in the pipeline.  The delegate is called by the previous delegate and in turn calls the next delegate.  With this pattern, a middleware can perform work before and after the call to the next delegate or skip the next delegate if appropriate.  Take the Help middleware for example. When `-h`, `--help` or `-?` been specified, the help middleware prints help for the target command and does not call the next middleware.

As the middleware executes, it enhances the _CommandContext_.  The _CommandContext_ contains the context relevate to the current command. For example... the tokenizer middleware populates the _CommandContext.Tokens_ property.  The parser middleware populates the _CommandContext.ParseResults_ property.  The _CommandContext_ is part of the delegate signature and is therefore available for all middleware.

### Why we use the middleware pattern
* Ease of adding new features without touching existing code. Observing [Single Resposibility](https://en.wikipedia.org/wiki/Single_responsibility_principle) &  [Open-Closed](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle) principals.
* Ease of extending the framework using external packages. Supports community enhancements while reducing the bloat of the framework code base.
* Feature tests can exclude middleware that's not relevant to the tests.

![Middleware](./../diagrams/MiddlewarePipeline.png){:style="float: right;"}

### Middleware Stages
The core functionality of the framework and optional features have been implemented using middleware. 
One of the classic challenges of using the middleware pattern is understanding the order middleware components should be registered. To address this challenge, the pipeline has been split into eight distinct stages, four core stages (in green) and four extensibility stages (in yellow). The four core stages contain the core functionality of the framework. After each core stage is complete, the framework guarantees specific parts of the _CommandContext_ are populated.

The other four stages are where other middleware will typically be registered, based on _CommandContext_ properties the middleware requires to operate. For example, _PromptForMissingOperands_ is registered in the _PostParseInputPreBindValues_ stage because it depends on the _ParseResults.ArgumentValues_ property being populated in the _ParseInput_ stage. _FluentValidation_ is registerd in the _PostBindValuesPreInvoke_ stage because it relies on the _IInvocation.ParameterValues_ being populated in the _BindValues_ stage.  Read more in the intellisense of _MiddlewareStages_ and the properties of the _CommandContext_ classes.

Note: _DisplayVersion_ is the only optional middleware in a core stage. Any middleware can be registered in a core stage if needed. In the case of _DisplayVersion_, the middleware must run before _DisplayHelp_ otherwise help would trigger for the root command when it is not executable.

!!! Warning
    Avoid registering middleware into the core stages. in future versions of the framework, middleware may be added or removed or the order within stage could change. These will not be considered breaking changes. Only add middleware in these stages if you understand the risk. Use the constants in `MiddlewareSteps` to understand the order of existing core middleware and add unit tests for these values to identify if when they change.


### Invocation Pipeline
The invocation pipeline is assembled during the _ParseInput_ stage, captured in _CommandContext.InvocationPipeline_ property and run during the _InvokeInvocationPipeline_ stage. The pipeline consists of [interceptor methods](interceptors.md) and the target command method.
