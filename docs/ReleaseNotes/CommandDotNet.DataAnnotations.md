# CommandDotNet.DataAnnotations

## 1.1.0

Added support for [Localization](../Localization/overview.md)

## 1.0.1

The previous implementation validated each argument value in isolation.
This is appropriate for parameters but prevents more complex validations for argument models.

This version will validate all arguments from a model within the context of the model so validations can reference multiple properties.

## 1.0.0

Introduced DataAnnotations for validation. See [DataAnnotations](../Arguments/data-annotations-validation.md)