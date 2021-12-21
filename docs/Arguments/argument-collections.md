# Collections

With CommandDotNet, you can define a single Operand collection and many Option collections.

Let's enhance our rocket launcher to visit multiple planets and specify a crew.

<!-- snippet: arguments_collections -->
<a id='snippet-arguments_collections'></a>
```c#
public void LaunchRocket(IConsole console,
        IEnumerable<string> planets,
        [Option('c', "crew")] string[] crew)
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Arguments_Collections.cs#L11-L15' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Help looks like...

<!-- snippet: arguments_collections_help -->
<a id='snippet-arguments_collections_help'></a>
```bash
$ mission-control.exe LaunchRocket --help
Usage: mission-control.exe LaunchRocket [options] <planets>

Arguments:

  planets (Multiple)  <TEXT>

Options:

  -c | --crew (Multiple)  <TEXT>
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_collections_help.bash#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections_help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This is how you pass multiple options and operands.

<!-- snippet: arguments_collections_exe -->
<a id='snippet-arguments_collections_exe'></a>
```bash
$ mission-control.exe LaunchRocket mars earth jupiter -c aaron -c alex
launching rocket
planets: mars,earth,jupiter
crew: aaron,alex
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_collections_exe.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

In this example the operands are specified first here and the options last. Order is not required.

Since operands are positional and options are not, order is not required. Options can be intermixed with operands in any order.

<!-- snippet: arguments_collections_exe_intermixed -->
<a id='snippet-arguments_collections_exe_intermixed'></a>
```bash
$ mission-control.exe LaunchRocket mars -c aaron earth -c alex jupiter
launching rocket
planets: mars,earth,jupiter
crew: aaron,alex
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_collections_exe_intermixed.bash#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections_exe_intermixed' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Operand Collections

Operands is similar to the c# `params` modifier. There can only be one operand collection and it must be the last operand defined. Otherwise it would be impossible to determine when the collection stopped and the next operand was provided.

When the [ArgumentSeparatorStrategy](../ArgumentValues/argument-separator.md) is `PassThru`, arguments passed after the `--` are not included in the operand collection.

<!-- snippet: arguments_collections_exe_argument_separator_passthru -->
<a id='snippet-arguments_collections_exe_argument_separator_passthru'></a>
```bash
$ mission-control.exe LaunchRocket mars -c alex -- additional args here
launching rocket
planets: mars
crew: alex
separated: additional,args,here
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_collections_exe_argument_separator_passthru.bash#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections_exe_argument_separator_passthru' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Option Collections

Since options are named, you can have multiple option collections. 

There are two ways to assign values to option collections.

The first is to specify the option multple times as shown in the example above with `-c`.

The second is to define a split character for the option `[Option(Split=',')]` and then you can provide multiple values delimited with the split character.

<!-- snippet: arguments_collections_exe_split_args_only -->
<a id='snippet-arguments_collections_exe_split_args_only'></a>
```bash
$ mission-control.exe LaunchRocket mars earth jupiter -c aaron,alex
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_collections_exe_split_args_only.bash#L1-L3' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections_exe_split_args_only' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Split can also be defined at a global level by setting `AppSettings.Arguments.DefaultOptionSplit`.  The default is null.

In cases where the user cannot use the provided split character (perhaps the script language does not support the character), the user can override it using the `[split]` directive.  For example, if the user would prefer a hyphen, they can use the directive where the last character is the delimiter to use, like this...

<!-- snippet: arguments_collections_exe_split_directive_args_only -->
<a id='snippet-arguments_collections_exe_split_directive_args_only'></a>
```bash
$ mission-control.exe [split:-] LaunchRocket mars earth jupiter -c aaron-alex
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/arguments_collections_exe_split_directive_args_only.bash#L1-L3' title='Snippet source file'>snippet source</a> | <a href='#snippet-arguments_collections_exe_split_directive_args_only' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Piping

Arguments can be piped into operand collections and can be routed to option collections by the user. Read more about it in [Piped arguments](../ArgumentValues/piped-arguments.md)

## Prompts

See [prompting for missing arguments](../ArgumentValues/prompting.md#prompting-for-missing-arguments) to see how prompting for collections works.

[Replace the default prompter](../ArgumentValues/prompting.md#prompting-from-within-the-command-method) to provide a different experience.
