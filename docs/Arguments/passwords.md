#Passwords

The `Password` type can be use to define arguments that contain confidential data.

`Password` has two features to the value from being leaked to logs and console output.

* the value is exposed via the `GetPassword()` method instead of a property so serializers cannot access the value.
* `ToString()` will output `*****` if there is a value, otherwise an empty string. This helps identify if a value was provided in logs but gives no indication how many characters, unless it happens to have 5 characters.

``` cs

public class Api
{
    public void Download(
        string url, 
        [Option] string username, 
        [Option] Password password) { ... }
}

```

## Prompting

When using `Password` with the built-in [prompting features](../ArgumentValues/prompting.md), password prompts will hide all characters.


## Caution

Best practice is to avoid passwords. Using `Password` is *only slightly* more secure than using a string. 
If the user provides a password as one of the input arguments, it may be logged via [parse token transformations](../Diagnostics/parse-directive.md#token-transformations) in some cases.
The raw values can still be accessed via `Environment.CommandLine` and `CommandContext.OriginalInput` and accidentally exposed in logging.

## Safer alternatives

* Use application tokens where possible, accessed via config files or environment variables.
* If a password is required, [prompt the user within the command](../ArgumentValues/prompting.md/#prompting-from-within-the-command-method). Use the 'isPassword' parameter to hide the input.
