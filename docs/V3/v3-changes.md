# v3 Changes

v3 is a significant rewrite of the CommandDotNet.

The framework has remained mostly backward compatible for basic scenarios. 
Anything beyond basic scenarios will encounter a breaking change.

We welcome everyones help in testing the changes, providing usability feedback, bugs, missing documentation, etc.

Given the scope of changes, it's likely something is missed in this doc. If you notice something, please create an issue or submit a pull request for the docs.  Thank you.

## What are the changes?

### New architecture
The architecture is not based around a [middleware pipeline](middleware.md).

### Arguments terminology
Terminology for arguments has been updated and made more consistent throughout the framework.  [This article](http://www.informit.com/articles/article.aspx?p=175771) describes how `argument` is an overloaded term.  It can mean all of the words after the name of the application name or just the words that are not commands.

We now recognize there are two different contexts with different needs.

1. The user of the console app: needs to understand when arguments are named vs positional. We should strive to use terms they are already familiar with.

2. The developer of the console app: needs to define arguments that are named vs positional. Occasionally the developers need to operate across both types of arguments regardless of the type.

We've addressed both. Now, there are two types of arguments; options and operands. And, there are two ways to define them; parameters and properties. Options are always named and Operands are always positional.  Pararmeter is no longer an overloaded term for argument.

Determining what should be an option vs operand can be confusing. One approach is to consider [operands](https://en.wikipedia.org/wiki/Operand) `what` the command operates on and options inform `how` the command operates on them, as described in the article from above.  For example, In the [calculator here](interceptors.md), `x` and `y` are the operands and `--radix` informs how the numbers are represented in the operations.

The user's interface with the app is the help documentation where we still use the terms command, option and argument. This is the more common terminology in help documentation of existing apps and so the user is likely more familiar with it. We don't expect users to understand what an operand is.

### Constructor based dependency injection is now possible
[read more below](#interceptor-methods)

## Summary of new features

* [Response file](response-files.md) support 
* `HelpTextProvider` can be overridden to make targetted changes to a section of help.
* Test tools, helpful for end-to-end test and testing framework extensions, like middleware components. (available soon)
* List operands can be populated from [piped arguments](piped-arguments.md).
* [Parameter resolvers](parameter-resolvers.md)
* [Ctrl+C support](cancellation.md) with CancellationToken 

Several bugs were fixed along the way.

## Breaking Changes

We initially tried to roll out this update in backward compatible phases. Due to the scope of changes, that proved more burdonsome that it was worth. We decided the benefits were worth the price of breaking changes. Hopefully you'll agree... maybe not during the update, but shortly there after. :smile:

In some cases, i.e. renamed attributes, the old method or class has been marked with `[Obsolete]` and warnings will suggest how to upgrade.  These will be removed in the next major release, `4.x`

There are a set of changes your IDE should be able to help you with.  For example, namespaces were updated to reflect their functional purpose. Your IDE will likely suggest the fixes and then you can remove the old namespaces. 

### AppSettings & Configuration

Fluent validation, prompting for missing arguments and fluent validation are no longer enabled by default.  Use these configuration extensions to enable them: `UseVersionMiddleware`, `UsePromptForMissingOperands`, and `UseFluentValidation`

### Interceptor Methods

#### Interceptor Methods replace Constructor Options 

Constructor-based dependency injection was not possible because of the feature using constructors to define options that can be used for all subcommands defined in that class prevented

Those constructors will need to be replaced with interceptor methods.  There are two signatures for interceptor methods.

`Task<int> Inteceptor(CommandContext ctx, ExecutionDelegate next, ...)`

and

`Task<int> Inteceptor(InterceptorExecutionDelegate next, ...)`

The method name does not matter.  What does matter is the `Task<int>` type and use of either `ExecutionDelegate`.  The former requires a `CommandContext`. Options can be defined in these methods but are not required.

#### Interceptor Methods scope includes all ancestor commands

Previously with constructor options, when there were multiple levels of subcommands, only the constructor option for the class of the target command was executed.
This can lead to a confusing experience where a parent command defines options but they aren't used because a subcommand of a subcommand has been requested.

As an example, let's say we have a console app `api.exe` for an api and the app mimics the RESTful design of the api. 

The commands are:

* `api.exe` - the root app, with a constructor defining the option `--url`
* `users` - command from a nested class
* `list` - a method to list the users

Usage: `api.exe --url api-url users ls`

This example would not work in v2 because the --url option, defined in the API class would never be called.  

v3 fixes this by keeping a pipeline of all interceptors.

If you need the old behavior, use the following code to keep only the interceptor in the same class as the target command. 

``` c#
appRunner.Configure(c => c.UseMiddleware((ctx, next) =>
{
    var pipeline = ctx.InvocationPipeline;
    pipeline.AncestorInterceptors = pipeline.All
        .Where(s => s.Invocation.MethodInfo.DeclaringType == pipeline.TargetCommand.Invocation.MethodInfo.DeclaringType)
        .ToList();
    return next(ctx);
}, MiddlewareStages.PostParseInputPreBindValues));
```

### Configuration

Use `AppRunner.Configure` for any configuration not located in AppSettings. This includes setting a custom help text provider, dependency resolver, etc.

### CommandInvoker removed
The middleware pipeline architecture obsoleted the CommandInvoker. Look at the `CommandInvokerTests` in the repo to see how the feature can be implemented using middleware. Where the CommandInvoker only worked for the command method, the new architecture allows interogating all of the interceptor methods too.

## Remaining work

* [ ] external dependencies
    * [ ] move FluentValidation into a separate nuget package
    * [ ] move Humanizer into a separate nuget package
    * [ ] update CommandDotNet.Ioc... repos
* [ ] make test tools available via nuget
* [ ] list parser takes 
* [ ] complete features
    * [ ] Cancellation middleware
    * [ ] RemainingOperands
    * [ ] SeparatedArguments
    * [ ] argument arity
        * [ ] define it with argument
        * [ ] display it in help
        * [ ] validate it