# [suggest] Directive

The `[suggest]` directive provides intelligent shell completion suggestions based on the current parse state. It's designed to support shell completion scripts by leveraging CommandDotNet's full parsing pipeline.

## Enabling the Directive

```c#
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

```c#
public class GitApp
{
    [Command]
    public void Commit([Option] string message) { }
    
    [Command]
    public void Push([Option] bool force) { }
}
```

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

```c#
public enum LogLevel { Debug, Info, Warning, Error }

public class App
{
    [Command]
    public void Log([Option] LogLevel level = LogLevel.Info) { }
}
```

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

```c#
public class App
{
    [Subcommand]
    public class Git
    {
        [Command]
        public void Commit([Option] string message) { }
        
        [Command]
        public void Push() { }
    }
}
```

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

The suggest directive is designed to be called by shell completion scripts:

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

1. **Always enable with directives**: `appRunner.UseDefaultMiddleware()` includes directive support
2. **Use with other middleware**: Suggest directive works alongside all other middleware
3. **Test completion scenarios**: Verify suggestions work for your command structure
4. **Consider AllowedValues**: Define allowed values for common parameters

## Limitations

- Suggestions are synchronous (no async suggestion sources)
- File path completion not built-in (use shell's native completion)
- No custom suggestion providers yet (coming in future releases)

## See Also

- [Directives Overview](../directives.md)
- [Middleware](../middleware.md)
- [Shell Completion (Advanced)](../shell-completions.md)
