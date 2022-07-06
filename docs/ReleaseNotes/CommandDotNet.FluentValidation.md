# CommandDotNet.FluentValidation

## 6.0.2

* support move of AppSettings.Localize to AppSettings.Localization
* ResourceProxy with memberNameAsKey, to better support resx files.

## 6.0.1

* update to dotnet 6

## 5.0.1

Ensure FluentValidation will run before arity is validated. FluentValidation could be more specific.

## 5.0.0

remove nuget package refs no longer required after move to net5.0

### Breaking Changes

Upgraded to latest FluentValidator 10.3.4 (from 8.0.0). 

The ValidatorAttribute was deprecated in v8 so we dropped support for it.
The assemblies containg the IArgModels will be scanned for validators if 
they aren't registered with a DI container.

Alternatively, you can provide a factory func via `.UseFluentValidation(validatorFactory:...)`

## 4.0.0

CommandDotNet.FluentValidation targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

## 3.1.0

Added support for [Localization](../Localization/overview.md)

## 3.0.1

Changed name of middleware method from ValidateModels to FluentValidationForModels. 
This will only be relevant when reviewing the stacktrace or printing the AppConfig.

## 3.0.0

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### MiddlewareSteps

add MiddlewareSteps.FluentValidation, declaring the stage and order within the stage the middleware is registered

## 2.0.3

remove extra NewLine when help will not be shown

## 2.0.2

Add `showHelpOnError` parameter to show help after validation errors

## 2.0.1

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```

## 2.0.0

Upgrade to FluentValidation 8.0
