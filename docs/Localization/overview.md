# Overview

## Beta feature
Coverage is not 100%. Help output and common use cases have been prioritized. Configuration errors for developers are not included at this time as it's assumed the app will be tested before being given to users who wouldn't be able to act on the errors anyway. We will accept PR's if this is important to you.

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

## Advanced cases

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

