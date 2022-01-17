# Piped arguments

## What is piping?
Piping is a way to pass the console output of a command to another command.

A simple example is filtering your history of bash commands for git commands: `history | grep git`.

The pipe `|` tells the shell you are running a new command and to send the output of the previous command to the new command.  
In this case, `history` will output the last N bash commands and `grep git` will filter the output to only the commands containing "git"

Piping is a great way to decompose commands into smaller, single-responsibility commands.

!!!tip
    When you find commands repeating the same options to determine what items to operate on, consider extracting the options into search command that returns ids and then update the other commands take the list of ids. 

    `app.exe get-customer-ids --only-active | app.exe migrate-customers`

In .net console apps, you can check piped input using `Console.IsInputRedirected` and `Console.In.Read___()`. 

With CommandDotNet, it's as simple as defining an operand as an enumerable type, or an option as an enumerable type and use use option targetting.

## How to capture piped input

### Operand collection

Every command is allowed a single operand collection. If one is defined and if piped input is available, it will be appended to this list. Here's an example.

<!-- snippet: piped_arguments -->
<a id='snippet-piped_arguments'></a>
```c#
public void List(IConsole console,
    [Option('i')] bool idsOnly)
{
    foreach (var user in userSvc.GetUsers())
        console.Out.WriteLine(idsOnly ? user.Id : user);
}

public void Disable(IConsole console, IEnumerable<string> ids)
{
    foreach (string id in ids)
    {
        if (userSvc.TryDisable(id, out var user))
            console.Out.WriteLine($"disabled {user}");
        else
            console.Out.WriteLine($"user not found {id}");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Values/Piped_Arguments.cs#L14-L32' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This app has two commands, one to list users and the other to disable them. The list command works like this...

<!-- snippet: piped_arguments_list -->
<a id='snippet-piped_arguments_list'></a>
```bash
$ users.exe List
a1 Avery (active)
b1 Beatrix (active)
c3 Cris (active)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/piped_arguments_list.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments_list' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

An option was defined to list just the ids and works like this...

<!-- snippet: piped_arguments_list_ids_only -->
<a id='snippet-piped_arguments_list_ids_only'></a>
```bash
$ users.exe List -i
a1
b1
c3
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/piped_arguments_list_ids_only.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments_list_ids_only' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This disabled commands takes a collection of ids and they can be piped to the disable command like this...

<!-- snippet: piped_arguments_disable -->
<a id='snippet-piped_arguments_disable'></a>
```bash
$ users.exe list -i | grep 1  | users.exe Disable c3
disabled c3 Cris (inactive)
disabled a1 Avery (inactive)
disabled b1 Beatrix (inactive)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/piped_arguments_disable.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments_disable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

What's happening here? The shell will start two processes, the first for List and the second for Disable. As the first process writes ids to the output stream, the id is "piped" to the second process. The Disable command will process the ids submitted in in the command (`c3`) and then will process the piped inputs as they are received. When List completes, the first process will shutdown and the pipe will be closed. The Disable command will complete any remaining ids and then shutdown.

Disable could define the ids parameter as a type of `List<string>` or `string[]`, but could not process any ids until the List command completed. By using `IEnumerable<string>`, the Disbabled command can process the ids as soon as the List command outputs them.

### Piping output to option collections

In cases where you want to direct piped output to an option instead of an operand collection, the user can pass `$*` as the value and the piped output will be submitted.

Let's add a Welcome command to our example app.

<!-- snippet: piped_arguments_options -->
<a id='snippet-piped_arguments_options'></a>
```c#
public void Welcome(IConsole console, string userId, [Option] ICollection<string> notify)
{
    console.Out.WriteLine($"welcome {userSvc.Get(userId)?.Name}");
    foreach (var user in notify
                 .Select(id => userSvc.Get(id))
                 .Where(u => u is not null && u.IsActive && u.Id != userId))
    {
        console.Out.WriteLine($"notify: {user}");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Values/Piped_Arguments.cs#L45-L56' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments_options' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

And let's welcome Cris and send a notification to the other users

<!-- snippet: piped_arguments_options_notify -->
<a id='snippet-piped_arguments_options_notify'></a>
```bash
$ users.exe list -i | users.exe Welcome c3 --notify ^
welcome Cris
notify: a1 Avery (active)
notify: b1 Beatrix (active)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/piped_arguments_options_notify.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments_options_notify' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice we used `$*` pipe the users ids to the --notify option.

You can choose a different symbol using `AppSettings.Arguments.DefaultPipeTargetSymbol`. The default is `$*`. Set to null to prevent users from directing to options.

If the user cannot use the symbols you've defined, they can override them using the `[pipeto:{new-symbols}]` directive.

<!-- snippet: piped_arguments_options_notify_pipeto_directive -->
<a id='snippet-piped_arguments_options_notify_pipeto_directive'></a>
```bash
$ users.exe list -i | users.exe [pipeto:***] Welcome c3 --notify ***
welcome Cris
notify: a1 Avery (active)
notify: b1 Beatrix (active)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/piped_arguments_options_notify_pipeto_directive.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-piped_arguments_options_notify_pipeto_directive' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Summary

Piping provides a powerful mechanism to break commands into smaller pieces and use the output from a command as input for other commands.

Some points to remember for CommandDotNet

* If the user pipes input and also specifies values for the operand, the two sources will be concatenated with piped input appended after the user defined input, as seen in the first example with c3.
* When the collection type is `IEnumerable<T>`, the values will be streamed to the enumerable as they're received from the publishing program, otherwise the command will have to wait until the publishing program is complete before it can begin processing.
* You can experiment with this using our [pipes example command](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Commands/Pipes.cs).
* If the collection argument receiving the piped input has an `Arity.Maximum < int.MaxValue`, CommandDotNet will process the input to the max arity and skip any remaining output.

Points to remember for piping in general

* All output to the console is piped to the next command. Be sure the command generating the piped output does not include additional logging information.
* Consider how much time is spent initialing the app and querying the data. While breaking the app into smaller pieces improves maintenance, testing and reusability, the performance impact may be prohibitive in some cases.
