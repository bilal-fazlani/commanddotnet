You can nest commands. Let's take `git` for example

`git` has a command called stash. When you execute `git stash`, it stashes all the changes. But stash has further commands like, `git stash pop`, `git stash list`, etc.

Let's try and mimic the same behavior using CommandDotNet:

```c#
[ApplicationMetadata(Description = "Fake git application")]
public class Git
{
    [ApplicationMetadata(Description = "Commits all staged changes")]
    public void Commit([Option(ShortName = "m")]string commitMessage)
    {
        Console.WriteLine("Commit successful");
    }

    [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments")]
    [SubCommand]
    public class Stash
    {
        [DefaultMethod]
        public void StashDefaultCommand()
        {
            Console.WriteLine($"changes stashed");
        }
    
        [ApplicationMetadata(Description = "Applies last stashed changes")]
        public void Pop()
        {
            Console.WriteLine($"stash popped");
        }

        [ApplicationMetadata(Description = "Lists all stashed changes")]
        public void List()
        {
            Console.WriteLine($"here's the list of stash");
        }
    }
}
```

Here's how the help looks like now:

```bash
Fake git application

Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information

Commands:
  Commit  Commits all staged changes
  Stash   Stashes all changes when executed without any arguments

Use "dotnet example.dll [command] --help" for more information about a command.
```

Here's  how the interaction looks like:

INPUT

```bash
dotnet example.dll commit -m "some refactoring"
```

OUTPUT

```bash
Commit successful
```

---

INPUT

```bash
dotnet example.dll stash
```

OUTPUT

```bash
changes stashed
```

---

INPUT

```bash
dotnet example.dll stash --help
```

OUTPUT

```bash
Stashes all changes when executed without any arguments

Usage: dotnet example.dll Stash [options] [command]

Options:
  -h | -? | --help  Show help information

Commands:
  List  Lists all saved stashed changes
  Pop   Applies last stashed changes

Use "Stash [command] --help" for more information about a command.
```

---

INPUT

```bash
dotnet example.dll stash pop
```

OUTPUT

```bash
stash popped
```

***Alternative***

If you like to store your sub commands as external `.cs` files, you can that too with `[SubCommand]` attribute.

```c#
    [ApplicationMetadata(Description = "Stashes all changes when executed without any arguments")]
    public class Stash
    {
        [ApplicationMetadata(Description = "Applies last stashed changes")]
        public void Pop()
        {
            Console.WriteLine($"stash popped");
        }
    }
```

```c#
[ApplicationMetadata(Description = "Fake git application")]
public class Git
{
    [SubCommand]
    public Stash Stash {get;set;} // Stash class is saved in a seperate file

    [ApplicationMetadata(Description = "Commits all staged changes")]
    public void Commit([Option(ShortName = "m")]string commitMessage)
    {
        Console.WriteLine("Commit successful");
    }
}
```