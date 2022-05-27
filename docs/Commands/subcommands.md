# Subcommands

You can nest commands. Let's take `git` for example

`git` has a command called stash. When you execute `git stash`, it stashes all the changes. But stash has further commands like, `git stash pop`, `git stash list`, etc.

Let's mimic the same behavior using CommandDotNet:

## Subcommand as a property

<!-- snippet: subcommands_git_composed -->
<a id='snippet-subcommands_git_composed'></a>
```c#
[Command(Description = "Fake git application")]
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);
    public static AppRunner AppRunner =>
        new AppRunner<Program>().UseNameCasing(Case.KebabCase);

    // Properties decorated with the [Subcommand] attribute will be subcommands of the host
    [Subcommand]
    public Stash Stash { get; set; }

    [Command(Description = "Commits all staged changes")]
    public void Commit(IConsole console, [Option('m', null)] string? commitMessage)
    {
        console.WriteLine("Commit successful");
    }
}

[Command(Description = "Stashes all changes when executed without any arguments")]
[Subcommand]
public class Stash
{
    [DefaultCommand]
    public void StashImpl(IConsole console)
    {
        console.WriteLine("changes stashed");
    }

    [Command(Description = "Applies last stashed changes")]
    public void Pop(IConsole console)
    {
        console.WriteLine("stash popped");
    }

    [Command(Description = "Lists all stashed changes")]
    public void List(IConsole console)
    {
        console.WriteLine("here's the list of stash");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Commands/Subcommands/Subcommands_Git_Composed.cs#L11-L52' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_composed' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the `Stash` property decorated with the `[SubCommand]` attribute.

The help will generate as:

<!-- snippet: subcommands_git_composed_help -->
<a id='snippet-subcommands_git_composed_help'></a>
```bash
$ git.exe -h
Fake git application

Usage: git.exe [command]

Commands:

  commit  Commits all staged changes
  stash   Stashes all changes when executed without any arguments

Use "git.exe [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/subcommands_git_composed_help.bash#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_composed_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Here's how the interaction looks like:

<!-- snippet: subcommands_git_composed_commit -->
<a id='snippet-subcommands_git_composed_commit'></a>
```bash
$ git.exe commit -m "some refactoring"
Commit successful
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/subcommands_git_composed_commit.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_composed_commit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: subcommands_git_composed_stash -->
<a id='snippet-subcommands_git_composed_stash'></a>
```bash
$ git.exe stash
changes stashed
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/subcommands_git_composed_stash.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_composed_stash' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: subcommands_git_composed_stash_help -->
<a id='snippet-subcommands_git_composed_stash_help'></a>
```bash
$ git.exe stash -h
Stashes all changes when executed without any arguments

Usage: git.exe stash [command]

Commands:

  list  Lists all stashed changes
  pop   Applies last stashed changes

Use "git.exe stash [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/subcommands_git_composed_stash_help.bash#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_composed_stash_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: subcommands_git_composed_stash_pop -->
<a id='snippet-subcommands_git_composed_stash_pop'></a>
```bash
$ git.exe stash pop
stash popped
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/subcommands_git_composed_stash_pop.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_composed_stash_pop' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

!!! Tip
    See [Nullable Reference Types](../TipsFaqs/nullable-reference-types.md) for avoiding  "Non-nullable property is uninitialized" warnings for subcommand properties

## Subcommand as a nested class

The same git stash command could be modelled as a nested class.

<!-- snippet: subcommands_git_nested -->
<a id='snippet-subcommands_git_nested'></a>
```c#
[Command(Description = "Fake git application")]
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);
    public static AppRunner AppRunner =>
        new AppRunner<Program>()
            .UseNameCasing(Case.KebabCase);

    [Command(Description = "Commits all staged changes")]
    public void Commit(IConsole console, [Option('m')] string? commitMessage)
    {
        console.WriteLine("Commit successful");
    }

    [Command(Description = "Stashes all changes when executed without any arguments")]
    [Subcommand]
    public class Stash
    {
        [DefaultCommand]
        public void StashImpl(IConsole console)
        {
            console.WriteLine("changes stashed");
        }

        [Command(Description = "Applies last stashed changes")]
        public void Pop(IConsole console)
        {
            console.WriteLine("stash popped");
        }

        [Command(Description = "Lists all stashed changes")]
        public void List(IConsole console)
        {
            console.WriteLine("here's the list of stash");
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Commands/Subcommands/Subcommands_Git_Nested.cs#L9-L47' title='Snippet source file'>snippet source</a> | <a href='#snippet-subcommands_git_nested' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
