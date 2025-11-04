# [suggest] Directive

The `[suggest]` directive provides intelligent shell completion suggestions based on the current parse state. It's designed to support shell completion scripts by leveraging CommandDotNet's full parsing pipeline.

## Enabling the Directive

The suggest directive is included when using `UseDefaultMiddleware()`:

```cs
appRunner.UseDefaultMiddleware();
```

Or enable it explicitly:

```cs
appRunner.UseSuggestDirective();
```

## How It Works

The suggest directive runs **after parsing** but before command execution. This means:

1. ✅ All middleware transformations are applied
2. ✅ Custom validation rules are honored
3. ✅ Type-specific completions (enums, allowed values) work automatically
4. ✅ Parse errors provide contextual suggestions

## Usage

```bash
myapp [suggest]                    # Suggests commands and options at root level
myapp [suggest] command            # Suggests subcommands and options for 'command'
myapp [suggest] command --opt      # Suggests values for '--opt'
myapp [suggest] command --opt par  # Filters suggestions starting with 'par'
```

## What Gets Suggested

### Commands & Subcommands
- All visible subcommands of the current command
- Commands starting with `__` are hidden

### Options
- All non-hidden options on the current command
- Middleware options (like `--help`) even if marked hidden
- Options that haven't been filled yet (respects arity)

### Argument Values
- **Enum parameters**: All enum values
- **AllowedValues**: Explicitly defined allowed values
- **Operands**: Values for the next unfilled operand

### Filtering
- Partial matches filter suggestions automatically
- Case-sensitive prefix matching

## Examples

### Basic Command Suggestions

<!-- snippet: suggest_basic_commands -->
<a id='snippet-suggest_basic_commands'></a>
```cs
public class GitApp
{
    [Command]
    public void Commit(string message) { }
    
    [Command]
    public void Push() { }
    
    [Command]
    public void Pull() { }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/Suggest_Examples.cs#L5-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-suggest_basic_commands' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

```bash
$ myapp [suggest]
# Output:
Commit
Push
--help

$ myapp [suggest] Co
# Output:
Commit
```

### Enum Value Completion

<!-- snippet: suggest_enum_values -->
<a id='snippet-suggest_enum_values'></a>
```cs
public enum LogLevel { Debug, Info, Warning, Error }

public class App
{
    public void SetLogLevel(LogLevel level)
    {
        System.Console.WriteLine($"Log level set to {level}");
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/Suggest_Examples.cs#L19-L29' title='Snippet source file'>snippet source</a> | <a href='#snippet-suggest_enum_values' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

```bash
$ myapp [suggest] Log --level
# Output:
Debug
Info
Warning
Error

$ myapp [suggest] Log --level Wa
# Output:
Warning
```

### Nested Subcommands

<!-- snippet: suggest_nested_subcommands -->
<a id='snippet-suggest_nested_subcommands'></a>
```cs
public class App2
{
    [Subcommand]
    public class Remote
    {
        public void Add(string name, string url) { }
        public void Remove(string name) { }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/OtherFeatures/Suggest_Examples.cs#L31-L41' title='Snippet source file'>snippet source</a> | <a href='#snippet-suggest_nested_subcommands' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

```bash
$ myapp [suggest] Git
# Output:
Commit
Push
--help

$ myapp [suggest] Git Commit
# Output:
--message
--help
```

## Integration with Shell Completion

!!! tip "Easy Setup with CompletionCommand"
    CommandDotNet provides a built-in `CompletionCommand` class that generates completion scripts for you. See [Shell Completions](shell-completions.md) for the recommended approach.

The suggest directive is designed to be called by shell completion scripts. If you need to write custom completion scripts, here are examples:

### Bash Example
```bash
_myapp_completions()
{
    local suggestions=$(myapp [suggest] "${COMP_WORDS[@]}" 2>/dev/null)
    COMPREPLY=($(compgen -W "$suggestions"))
}

complete -F _myapp_completions myapp
```

### Zsh Example
```zsh
_myapp_completions()
{
    local suggestions=(${(f)"$(myapp [suggest] ${words[@]} 2>/dev/null)"})
    _describe 'myapp commands' suggestions
}

compdef _myapp_completions myapp
```

## Parse Error Handling

The directive provides intelligent suggestions even when there are parse errors:

| Error Type | Suggestion Behavior |
|------------|-------------------|
| `UnrecognizedOption` | Suggests valid options for the command |
| `UnrecognizedArgument` | Suggests valid subcommands |
| `NotAllowedValue` | Suggests allowed values for the argument |
| `MissingOptionValue` | Suggests allowed values for the option |

## Best Practices

1. **Use default middleware**: `appRunner.UseDefaultMiddleware()` includes the suggest directive
2. **Use with other middleware**: Suggest directive works alongside all other middleware
3. **Test completion scenarios**: Verify suggestions work for your command structure
4. **Consider AllowedValues**: Define allowed values for common parameters

## Limitations

- Suggestions are synchronous (no async suggestion sources)
- File path completion not built-in (use shell's native completion)
- No custom suggestion providers yet (coming in future releases)

## See Also

- [Shell Completions](shell-completions.md) - Generate completion scripts with `CompletionCommand`
- [Directives](../Extensibility/directives.md) - Understanding directives
- [Middleware](../Extensibility/middleware.md) - Extensibility pipeline
