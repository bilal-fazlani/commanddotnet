# Piped Arguments

## What is piping?
Piping is a way to pass the console output of a command to another another command.

A simple example is searching the history of your bash commands for git commands: `history | grep git`.

The pipe `|` tells the shell you are running a new command and that the new command will take the output of the previous command.  
In this case, `history` will output the last N bash commands and `grep git` will filter the output to only the commands containing "git"

Piping is a great way to decompose commands into smaller, single-responsibility commands.

!!!tip
    When you find commands repeating the same options to determine what items to operate on, consider extracting the options into search command that returns ids and then update the other commands take the list of ids.

In .net console apps, you can check piped input using `Console.IsInputRedirected` and `Console.In.Read___()`. 

Or... use our provided middleware

## Using the middleware?

First, enable the feature with `appRunner.AppendPipedInputToOperandList()`.

Every command is allowed a single operand list. If one is defined and if piped input is available, it will be appended to this list. If the user provides values for the operand and pipes input, the two sources will be concatenated, with piped input at the end.

!!!tip
    All output to the console is piped to the next command. Be sure the command does not include additional logging information.

!!!caveat
    The middleware will currently read all piped input before processing the command. This will require the first command to completely finish before the second command starts so this is not yet suitable for streaming.  We hope to resolve this in a future release. If you need streaming, please consider helping us enhance this middleware.



