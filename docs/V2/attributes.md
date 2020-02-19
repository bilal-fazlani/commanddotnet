### ApplicationMetadata

You can use the `[ApplicationMetadata]` attribute on the class level like this to provide details when application is called with `help` switch.

Example: 

```c#
[ApplicationMetadata(Description = "This is a crappy calculator", ExtendedHelpText = "Some more help text that appears at the bottom")]
public class Calculator
{
}
```

This attribute can also be used on a Method as shown below.

```c#
[ApplicationMetadata(Description = "Subtracts value2 from value1 and prints output", 
    ExtendedHelpText = "Again, some more detailed help text which has no meaning I still have to write to demostrate this feature",
    Name = "subtractValues")]
public void Subtract(int value1, int value2)
{
}
```

!!! tip
    When you use ApplicationMetadata attribute on a method, you can change the name of the command that is different from method name.

### SubCommand

`[SubCommand]` attribute indicates that targeted property is a SubCommand.

See [Nesting commands](./nesting-commands.md) for examples

### Argument

Every parameter in the method is argument by default. So this this Attribute is optional and should be used only when you need to assign a different name to parameter, or add description to it.

By default, the parameter names declared in method are the argument names that appear in help. However you can change that.

Let's see an example-

```c#
public void LaunchRocket([Operand(
    Name = "planet",
    Description = "Name of the planet you wish the rocket to go")] string planetName)
```
This is what help looks like-

```bash
Usage: dotnet CommandDotNet.Example.dll LaunchRocket [arguments]

Arguments:

  planet    <TEXT>
  Name of the planet you wish the rocket to go
```

### Option

Every parameter in the method is argument by default. So if you wan't to turn a parameter into option instead of argument, use this attribute. See more info about parameters [here](#parameters)

By default, the parameter names declared in method are the option names that appear in help. However you can change that. By convention, an option can have a short name and/or a long name.

Let's see an example-

```c#
public void LaunchRocket([Option(
    Name = "planet", 
    ShortName = "p", 
    Description = "Name of the planet you wish the rocket to go")] string planetName)
```

This is what help looks like-

```bash
Usage: dotnet CommandDotNet.Example.dll LaunchRocket [options]

Options:

  -p|--planet    <TEXT>
  Name of the planet you wish the rocket to go
```

So planet name can now be passed either with `--planet` or `-p`. 
LongName, ShortName and Description are optional. 

When you don't specify a long name and a short name for an option, it uses the method parameter name by default as long name. In case the method parameter name is just one letter, it will be treated as short name.

Here's table of examples:

| Method parameter name | Longname | Shortname | Generated template
| --- | --- | --- | --- |
| planet |  |  | `--planet` |
| planet | planet |  | `--planet` |
| planet |  | p | `-p` |
| planet | planet | p | `-p | --planet` |
| p |  |  | `-p` |
