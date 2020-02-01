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

When using `Password` with the built-in [prompting features](prompting.md), password prompts will hide all characters.