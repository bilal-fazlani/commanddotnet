# Testing

## How to keep up with changes to the resources?

### How we do it

To ensure a `ResourcesProxy` overrides all members of it's base `Resources` class, we've added these [ResourceProxyTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/UnitTests/ResourceProxyTests.cs) which we can also use to regenerate a proxy when new members have been added to it's base.

### How you could do it

Using the patterns displayed in `ResourceProxyTests`, generate the translation files (resx, json, xliff). Then set up a test to compare previous with current.

## ResourcesDef
`ResourcesDef` is a class we've used to generate proxy classes and localization files. If you've built custom middleware following the same pattern used in CommandDotNet, you can use this class to do the same.

For this proxy pattern to work, all method parameters must be strings which allows the proxy to pass in place holders such as `{0}` and `{1}` for use in the `IStringLocalizer`.  Use the `ResourcesDef.Validate` method in unit tests to catch when a non-string parameter is used.

Use `ResourcesDef.IsMissingMembersFrom` in unit tests to detect when changes to the base resources class have not been added to the proxy.
### Generate your proxy classes

Follow the pattern in the [ResourceGenerators](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/UnitTests/Localization/ResourceGenerators.cs).`RegenerateProxyClasses` method to generate your proxy classes.

