<img src="./images/logo.png" width="250px" />

![Nuget](https://img.shields.io/nuget/v/commanddotnet?style=for-the-badge)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/CommandDotNet.svg?style=for-the-badge)](https://www.nuget.org/packages/CommandDotNet)
[![NuGet](https://img.shields.io/nuget/dt/CommandDotNet.svg?style=for-the-badge)](https://www.nuget.org/packages/CommandDotNet)
[![GitHub](https://img.shields.io/github/license/bilal-fazlani/commanddotnet?style=for-the-badge)](https://github.com/bilal-fazlani/commanddotnet/blob/master/LICENSE)

[![GitHub last commit](https://img.shields.io/github/last-commit/bilal-fazlani/CommandDotNet.svg?style=for-the-badge)]()
![Netlify](https://img.shields.io/netlify/ce6331f7-bbfb-4a8a-ba7c-705b2902c4f5?label=Netlify%20Build&style=for-the-badge)
[![Build](https://img.shields.io/github/workflow/status/bilal-fazlani/commanddotnet/Test/master?style=for-the-badge)](https://github.com/bilal-fazlani/commanddotnet/actions/workflows/test.yml)

[![Gitter](https://img.shields.io/gitter/room/badges/shields.svg?style=for-the-badge)](https://gitter.im/CommandDotNet/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Discord](https://img.shields.io/discord/678568687556493322?label=Discord%20Chat&style=for-the-badge)](https://discord.gg/QFxKSeG)

# CommandDotNet

### A modern framework for building modern CLI apps

Out of the box support for commands, sub-commands, validations, dependency injection, 
piping and streaming, enums & custom types, typo suggestions, prompting, passwords, response files and much more! 
See the [features page](https://commanddotnet.bilal-fazlani.com/features). 

Favors [POSIX conventions](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html)

Includes [test tools](TestTools/overview.md) used by the framework to test all features of the framework.

Modify and extend the functionality of the framework through configuration and middleware.

### Documentation 👉 https://commanddotnet.bilal-fazlani.com

## Support

For bugs, [create an issue](https://github.com/bilal-fazlani/commanddotnet/issues/new)

For questions and feature requests, start [a discussion](https://github.com/bilal-fazlani/commanddotnet/discussions)

### Example

```c#
public class Calculator
{
    public void Add(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 + value2}");
    }

    public void Subtract(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 - value2}");
    }
}
```

```c#
class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Calculator>().Run(args);
    }
}
```

With these two classes, we've defined an Add and Subtract command and help is automatically generated.

```bash
~
$ dotnet calc.dll -h
Usage: dotnet calc.dll

Usage: dotnet calc.dll [command]

Commands:

  Add
  Subtract

Use "dotnet calc.dll [command] --help" for more information about a command.

~
$ dotnet calc.dll Add -h
Usage: dotnet calc.dll Add [arguments]

Arguments:

  value1  <NUMBER>

  value2  <NUMBER>

~
$ dotnet calc.dll Add 40 20
Answer: 60
```

Check out the docs for more examples

## Credits 🎉

Special thanks to [Drew Burlingame](https://github.com/drewburlingame) for continuous support and contributions
