# Overview

## Beta feature
Coverage is not 100%. Help output and common use cases have been prioritized. Configuration errors for developers are not included at this time as it's assumed the app will be tested before being given to users who wouldn't be able to act on the errors anyway. We will accept PR's if this is important to you.

Submit feedback via [GitHub Issues](https://github.com/bilal-fazlani/commanddotnet/issues), [GitHub Discussions](https://github.com/bilal-fazlani/commanddotnet/discussions) or [![Discord](https://img.shields.io/discord/678568687556493322?label=Discord%20Chat&style=for-the-badge)](https://discord.gg/QFxKSeG)

We will also need help with translations and will accept PRs for those translations.

## TLDR, How to enable 
Enable the feature by setting `appSettings.Localize` to a `Func<string,string?>` such as `text => stringLocalizer[text]`.
Every package that supports localization will detect this during registration.

<!-- snippet: localization_basic_setup -->
<a id='snippet-localization_basic_setup'></a>
```cs
public static int BasicLocalizationSetup(string[] args)
{
    // Simple localization function - in real apps, this would call IStringLocalizer
    Func<string, string?> localizer = text => 
    {
        // Your localization logic here
        // e.g., stringLocalizer[text]
        return text; // Fallback to original text
    };
    
    var settings = new AppSettings
    {
        Localization = { Localize = localizer }
    };
    
    return new AppRunner<ValidationApp>(settings).Run(args);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Localization/Localization_Examples.cs#L13-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-localization_basic_setup' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Advanced cases

To use a different `Func<string,string?>` per package, supply the appropriate `ResourcesProxy` in the `AppRunner` constructor and in the registration for each package.

<!-- snippet: localization_resources_proxy -->
<a id='snippet-localization_resources_proxy'></a>
```cs
public static int UsingResourcesProxy(string[] args)
{
    // Different localizers for different packages
    Func<string, string?> coreLocalizer = text => text;      // Core framework
    Func<string, string?> validationLocalizer = text => text; // Validation messages
    
    return new AppRunner<ValidationApp>(
            new AppSettings(), 
            new ResourcesProxy(coreLocalizer))
        // .UseDataAnnotationValidations(
        //     new DataAnnotations.ResourcesProxy(validationLocalizer))
        .Run(args);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Localization/Localization_Examples.cs#L33-L47' title='Snippet source file'>snippet source</a> | <a href='#snippet-localization_resources_proxy' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To use a more custom approach, override the appropriate `Resources` class in each package and supply them during registration.

<!-- snippet: localization_custom_resources -->
<a id='snippet-localization_custom_resources'></a>
```cs
// Custom Resources class to override specific messages
public class MyResources : Resources
{
    public override string Error_File_not_found(string fullPath) => 
        $"Archivo no encontrado: {fullPath}"; // Spanish: "File not found"
    
    public override string Command_help => "ayuda"; // Spanish: "help"
    
    public override string Help_Usage => "Uso"; // Spanish: "Usage"
}

public static int UsingCustomResources(string[] args)
{
    return new AppRunner<ValidationApp>(
        new AppSettings(), 
        new MyResources()).Run(args);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Localization/Localization_Examples.cs#L49-L67' title='Snippet source file'>snippet source</a> | <a href='#snippet-localization_custom_resources' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
### Resx files & UseMemberNamesAsKeys

By default, the keys passed to the `AppSettings.Localization.Localize` delegate
are the values define in the Resources class.<br/>

Setting this to true will use the property or method names instead of the values.

Example: for property - `Common_argument_lc => "argument"` the default key is "argument"

When `AppSettings.Localization.UseMemberNamesAsKeys` is set to true, "Common_argument_lc" is the key.

Given localization values may differ only by case, for example "argument" vs "Argument", 
the VS resx editor will encounter errors if the resx file uses the values for keys.
We recommend using the member names as keys instead, as demonstrated in these 
auto-generated [resx localization files](https://github.com/bilal-fazlani/commanddotnet/tree/master/localization_files/resx/en).

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

There are examples in [ResourceGenerators](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/UnitTests/Localization/ResourceGenerators.cs)

Basic resx and json translations files can be found in the [localization_files](https://github.com/bilal-fazlani/commanddotnet/tree/master/localization_files) folder. 

!!! Warning
    Some keys differ only in case, eg. `Arguments` & `arguments`, which is not supported by some tools, such as the VS resx editor. This is why we use `AppSettings.Localization.UseMemberNamesAsKeys=true` for resx files.

If you'd like to see another format supported, submit a PR generating the file in `ResourceGenerators` and it will be updated when new keys are added.
