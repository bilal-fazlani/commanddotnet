Let's enhance our rocket launcher to support multiple planets.

### Collection of Options

```c#
public void LaunchRocket([Option(ShortName = "p")] List<string> planets)
```

This is what help information looks like-

INPUT

```bash
dotnet example.dll LaunchRocket --help
```

OUTPUT

```bash
Usage: dotnet CommandDotNet.Example.dll LaunchRocket [options]

Options:

  -p|--planets (Multiple)    <TEXT>
```

And this is how you pass multiple options:

```bash
dotnet example.dll LaunchRocket -p mars -p earth -p jupiter
```

### Collection of Arguments

```c#
public void LaunchRocket(List<string> planets)
```

INPUT

```bash
dotnet example.dll LaunchRocket --help
```

OUTPUT

```bash
Usage: dotnet CommandDotNet.Example.dll LaunchRocket [arguments]

Arguments:

  planets (Multiple)    <TEXT>
```

And this is how you pass multiple arguments:

```bash
dotnet example.dll LaunchRocket mars earth jupiter
```
