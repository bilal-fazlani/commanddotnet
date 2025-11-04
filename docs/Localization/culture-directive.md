# Culture Directive

## TLDR, How to enable 
Enable the feature with `appRunner.UseLocalizeDirective()`.

!!! Note
    The method is called `UseLocalizeDirective()` but the directive syntax is `[culture:name]`.

## Culture Directive

When the `[culture:name]` [directive](../Extensibility/directives.md) is used, the `CultureInfo.CurrentCulture` is set to the specified culture.

```bash
$ dotnet example.dll [culture:en-gb] Add 1 2
```
