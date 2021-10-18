# CommandDotNet.FluentValidation

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