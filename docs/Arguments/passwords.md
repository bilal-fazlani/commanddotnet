#Passwords

The `Password` type can be use to define arguments that contain confidential data.

`Password` has two features to the value from being leaked to logs and console output.

* the value is exposed via the `GetPassword()` method instead of a property so serializers cannot access the value.
* `ToString()` will output `*****` if there is a value, otherwise an empty string. This helps identify if a value was provided in logs but gives no indication how many characters, unless it happens to have 5 characters.

<!-- snippet: passwords_login -->
<a id='snippet-passwords_login'></a>
```c#
public void Login(IConsole console, string username, Password password)
{
    console.WriteLine($"u:{username} p:{password}");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Passwords.cs#L15-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-passwords_login' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: passwords_login_exe -->
<a id='snippet-passwords_login_exe'></a>
```bash
$ myapp.exe Login roy rogers
u:roy p:*****
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/passwords_login_exe.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-passwords_login_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Prompting

When using `Password` with the built-in [prompting features](../ArgumentValues/prompting.md), password prompts will hide all characters.

<!-- snippet: passwords_prompt -->
<a id='snippet-passwords_prompt'></a>
```c#
public void Prompt(IConsole console, IPrompter prompter, string username)
{
    var password = prompter.PromptForValue("password", out _, isPassword: true);
    console.WriteLine($"u:{username} p:{password}");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Passwords.cs#L22-L28' title='Snippet source file'>snippet source</a> | <a href='#snippet-passwords_prompt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: passwords_prompt_exe -->
<a id='snippet-passwords_prompt_exe'></a>
```bash
$ myapp.exe Prompt roy
password: 
u:roy p:rogers
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/passwords_prompt_exe.bash#L1-L5' title='Snippet source file'>snippet source</a> | <a href='#snippet-passwords_prompt_exe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice the 'isPassword' optional parameter hides the input

## Caution

Best practice is to avoid passwords. Using `Password` is *only slightly* more secure than using a string. 
If the user provides a password as one of the input arguments, it may be logged via [parse token transformations](../Diagnostics/parse-directive.md#token-transformations) in some cases.
The raw values can still be accessed via `Environment.CommandLine` and `CommandContext.OriginalInput` and accidentally exposed in logging. Prefer prompting over arguments when possible.

## Safer alternatives

* Use application tokens where possible, accessed via config files or environment variables.
* If a password is required, [prompt the user within the command](../ArgumentValues/prompting.md/#prompting-from-within-the-command-method).
