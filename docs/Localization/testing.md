# Testing

## ResourcesDef
`ResourcesDef` is a class we've used to generate the ResourceProxy classes. If you've built custom middleware following the same pattern used in CommandDotNet, you can use this class to generate your proxy classes.

Also use this in unit tests to detect when members of the Resources base class are not overriden. This will help identify missing overrides for new resources after updates of CommandDotNet.

## more to come