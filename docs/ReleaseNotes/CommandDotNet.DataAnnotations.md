# CommandDotNet.DataAnnotations

## 3.0.2

* support move of AppSettings.Localize to AppSettings.Localization
* ResourceProxy with memberNameAsKey, to better support resx files.

## 3.0.1

* update to dotnet 6

## 2.0.3

Ensure DataAnnotations will run before arity is validated. DataAnnotations could be more specific.

## 2.0.2

remove nuget package refs no longer required after move to net5.0

## 2.0.1

CommandDotNet.DataAnnotations targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

## 1.1.0

Added support for [Localization](../Localization/overview.md)

## 1.0.1

The previous implementation validated each argument value in isolation.
This is appropriate for parameters but prevents more complex validations for argument models.

This version will validate all arguments from a model within the context of the model so validations can reference multiple properties.

## 1.0.0

Introduced DataAnnotations for validation. See [DataAnnotations](../ArgumentValidation/data-annotations-validation.md)
