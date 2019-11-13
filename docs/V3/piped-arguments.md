# Piped Arguments

## What is piping?
Piping is a way to pass the console output of a command to another command.

A simple example is filtering your history of bash commands for git commands: `history | grep git`.

The pipe `|` tells the shell you are running a new command and that the new command will take the output of the previous command.  
In this case, `history` will output the last N bash commands and `grep git` will filter the output to only the commands containing "git"

Piping is a great way to decompose commands into smaller, single-responsibility commands.

!!!tip
    When you find commands repeating the same options to determine what items to operate on, consider extracting the options into search command that returns ids and then update the other commands take the list of ids. 

    `app.exe get-customer-ids --only-active | app.exe migrate-customers`

In .net console apps, you can check piped input using `Console.IsInputRedirected` and `Console.In.Read___()`. 

Or... use the `AppendPipedInputToOperandList` middleware

## Using the middleware?

First, enable the feature with `appRunner.AppendPipedInputToOperandList()`.

Every command is allowed a single operand list. If one is defined and if piped input is available, it will be appended to this list. If the user pipes input and also specifies values for the operand, the two sources will be concatenated with piped input at the end.

!!!tip
    All output to the console is piped to the next command. Be sure the command generating the piped output does not include additional logging information.

 