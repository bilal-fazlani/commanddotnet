# Subcommands

You can nest commands. Let's take `git` for example

`git` has a command called stash. When you execute `git stash`, it stashes all the changes. But stash has further commands like, `git stash pop`, `git stash list`, etc.

Let's mimic the same behavior using CommandDotNet:

```c#
[Command(Description = "Fake git application")]
public class Git
{
    [SubCommand]
    public Stash Stash {get;set;} // Stash class is saved in a seperate file

    [Command(Description = "Commits all staged changes")]
    public void Commit([Option(ShortName = "m")]string commitMessage)
    {
        Console.WriteLine("Commit successful");
    }
}

[Command(Description = "Stashes all changes when executed without any arguments")]
public class Stash
{
    [DefaultMethod]
    public void StashDefaultCommand()
    {
        Console.WriteLine($"changes stashed");
    }

    [Command(Description = "Applies last stashed changes")]
    public void Pop()
    {
        Console.WriteLine($"stash popped");
    }

    [Command(Description = "Lists all stashed changes")]
    public void List()
    {
        Console.WriteLine($"here's the list of stash");
    }
}
```

Here's how the help looks like now:

```bash
Fake git application

Usage: git.exe [command] [options]

Commands:
  Commit  Commits all staged changes
  Stash   Stashes all changes when executed without any arguments

Use "git.exe [command] --help" for more information about a command.
```

Here's how the interaction looks like:

```bash
~
$ git.exe commit -m "some refactoring"

Commit successful

~
$ git.exe stash

changes stashed

~
$ git.exe stash --help

Stashes all changes when executed without any arguments

Usage: git.exe Stash [command] [options]

Commands:
  List  Lists all saved stashed changes
  Pop   Applies last stashed changes

~
$ git.exe stash pop

stash popped
```

## Subcommand as a nested class

The same git stash command could be modelled as a nested class.

```c#
[Command(Description = "Fake git application")]
public class Git
{
    [Command(Description = "Commits all staged changes")]
    public void Commit([Option(ShortName = "m")]string commitMessage)
    {
        Console.WriteLine("Commit successful");
    }

    [Command(Description = "Stashes all changes when executed without any arguments")]
    [SubCommand]
    public class Stash
    {
        [DefaultMethod]
        public void StashDefaultCommand()
        {
            Console.WriteLine($"changes stashed");
        }
    
        [Command(Description = "Applies last stashed changes")]
        public void Pop()
        {
            Console.WriteLine($"stash popped");
        }

        [Command(Description = "Lists all stashed changes")]
        public void List()
        {
            Console.WriteLine($"here's the list of stash");
        }
    }
}
```