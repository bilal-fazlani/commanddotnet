# Name Casing

Use [Humanizer](https://github.com/Humanizr/Humanizer) with CommandDotNet to convert the case of command and arguments to PascalCase, camelCase, lowercase or kebab-case.

## TLDR, How to enable 

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.NameCasing
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.NameCasing
    ```
Enable the feature with `appRunner.NameCasing(...)`

## Case options

```c#
public class App
{
    public void MigrateUser([Option]bool dryRun){...}
}
```

With `Case.DontChange`, the command is executed as `MigrateUser --dryRun`

With `Case.PascalCase`, the command is executed as `MigrateUser --DryRun`

With `Case.CamelCase`, the command is executed as `migrateUser --dryRun`

With `Case.KebabCase`, the command is executed as `migrate-user --dry-run`

With `Case.LowerCase`, the command is executed as `migrateuser --dryrun`

## Overridden names

By default, the case is only applied where the name has not been overridden in an attribute.

```c#
public class App
{
    [Command("migrateUser")]
    public void MigrateUser([Option("DryRun")]bool dryRun){...}
}
```

the command is executed as `migrateUser --DryRun`.

Use the `applyToNameOverrides` option to apply case conversion to migrateUser and DryRun. 

!!!Caveats
    Humanizer behavior

    * Lowercase cannot be converted to another case... except, the first letter will be capitalized for Pascal. Humanizer doesn't know where the second word starts.
    * Kebabcase cannot be converted to camelcase or lowercase. No idea why.
    * Camel and Pascal can be converted to any other case

## Custom Name Transforamtions

use `appRunner.Configure(b => b.NameTransformation = ...)` to apply custom name transformations.

Example: [Humanizer middleare](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.NameCasing/HumanizerAppRunnerExtensions.cs)

<!-- snippet: name_casing_transformation -->
<a id='snippet-name_casing_transformation'></a>
```c#
/// <summary>Change the case of argument and command names to match the given cases</summary>
/// <param name="appRunner"></param>
/// <param name="case">The case to apply</param>
/// <param name="applyToNameOverrides">Case should be applied to names overridden in attributes.</param>
public static AppRunner UseNameCasing(this AppRunner appRunner, Case @case, bool applyToNameOverrides = false)
{
    return applyToNameOverrides
        ? appRunner.Configure(b => b.NameTransformation = (_, memberName, nameOverride, _) =>
            (nameOverride ?? memberName).ChangeCase(@case))
        : appRunner.Configure(b => b.NameTransformation = (_, memberName, nameOverride, _) =>
            nameOverride ?? memberName.ChangeCase(@case));
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.NameCasing/HumanizerAppRunnerExtensions.cs#L7-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-name_casing_transformation' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
