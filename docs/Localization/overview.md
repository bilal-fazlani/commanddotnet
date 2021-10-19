# Overview

## Beta feature
Coverage is not 100%. Help output and common use cases have been prioritized. Configuration errors for developers are not included at this time as it's assumed the app will be tested before being given to users who wouldn't be able to act on the errors anyway. We will accept PR's if this is important to you.

Submit feedback via [GitHub Issues](https://github.com/bilal-fazlani/commanddotnet/issues), [GitHub Discussions](https://github.com/bilal-fazlani/commanddotnet/discussions) or [![Discord](https://img.shields.io/discord/678568687556493322?label=Discord%20Chat&style=for-the-badge)](https://discord.gg/QFxKSeG)

We will also need help with translations and will accept PRs for those translations.

## TLDR, How to enable 
Enable the feature by setting `appSettings.Localize` to a `Func<string,string?>` such as `text => stringLocalizer[text]`.
Every package that supports localization will detect this during registration.

```c#
static int Main(string[] args)
{
    IStringLocalizer localizer = ConfigureLocalizer();
    var settings = new AppSettings{ Localization = t => localizer[t] };
    return new AppRunner<ValidationApp>(settings).Run(args);
}
```

### Advanced cases

To use a different `Func<string,string?>` per package, supply the appropriate `ResourcesProxy` in the `AppRunner` constructor and in the registration for each package.

```c#
static int Main(string[] args)
{
    IDictionary<string,IStringLocalizer> localizers = ConfigureLocalizers();
    var proxy = new ResourcesProxy{ Localization = t => localizers["core"][t] };
    return new AppRunner<ValidationApp>(settings, 
            new ResourcesProxy{ Localization = t => localizers["core"][t] })
        .UseDataAnnotationValidations(args, 
            new DataAnnotations.ResourcesProxy{ Localization = t => localizers["validation"][t] })
        .Run(args);
}
```

To use a more custom approach, override the appropriate `Resources` class in each package and supply them during registration.
```c#
class MyResources : Resources
{
    public override string Error_File_not_found(string fullPath) => $"missing file: {fullPath}";
    ...
}

static int Main(string[] args)
{
    return new AppRunner<ValidationApp>(settings, new MyResources()).Run(args);
}
```

## Implementation

### Goals

* provide localization 
* easy to use
* extensible 
* avoid the cost of translation lookups when not needed
* common entries available for reuse in other middleware
* easy to identify new entries
* fallback to default when localized values are not found

### Resources and ResourcesProxy

Every package that contains localizable content will contain, in the root namespace, a [Resources](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Resources.cs) class and a [ResourcesProxy](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/ResourcesProxy.cs) that takes a `Func<string,string>`.  Each package will have it's own implementation of `Resources` and `ResourcesProxy`.  For example, [DataAnnotations](https://github.com/bilal-fazlani/commanddotnet/tree/master/CommandDotNet.DataAnnotations) and [FluentValidation](https://github.com/bilal-fazlani/commanddotnet/tree/master/CommandDotNet.FluentValidation) have their own versions.

As seen in the examples above, the [IStringLocalizer indexer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.localization.istringlocalizer.item) can be used for `Func<string,string?>`, making this approach extensible without taking a dependency on localization extensions.

We chose to use a class instead of interface for `Resources` to avoid every addition being a breaking change. Localization should not cause your app to fail and should not force you to recompile code for every update. We've provided tooling to help you identify when new resources are added via unit testing.

### Testing 

See [Localization testing](testing.md) to see how we ensure proxies include every member and how you can generate and test your own.

See the [culture directive](culture-directive.md) for a way to set the culture for a command. Enable for unit tests and manual testing.

## Generating Resource Files

There are examples in the [ResourceProxyTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/UnitTests/ResourceProxyTests.cs)