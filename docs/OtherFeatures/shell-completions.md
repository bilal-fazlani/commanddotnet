# Shell Completions

CommandDotNet provides built-in support for generating shell completion scripts for bash, zsh, fish, and PowerShell. This enables tab completion for commands, options, and argument values in your CLI applications.

## Quick Start

Add the `CompletionCommand` class as a subcommand to your application:

<!-- snippet: shell_completions_quick_start -->
<a id='snippet-shell_completions_quick_start'></a>
```cs
public class MyApp
{
    [Subcommand]
    public CompletionCommand Completion { get; set; } = new();
    
    // your other commands...
    public void Deploy(string environment) { }
}

class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<MyApp>()
            .UseDefaultMiddleware()  // includes [suggest] directive
            .Run(args);
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Completions/Shell_Completions.cs#L11-L30' title='Snippet source file'>snippet source</a> | <a href='#snippet-shell_completions_quick_start' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

That's it! Your app now has a `completion` command that generates shell-specific completion scripts.

## Usage

### Generate Completion Scripts

Users can generate completion scripts for their shell:

```bash
# See available shells
myapp completion --help

# Generate bash completion
myapp completion bash

# Generate zsh completion
myapp completion zsh

# Generate fish completion
myapp completion fish

# Generate PowerShell completion
myapp completion powershell
```

### Installation Instructions

Each shell command includes installation instructions in its help text:

```bash
myapp completion bash --help
```

## Shell-Specific Installation

### Bash

**Option 1: Source directly in `.bashrc`**
```bash
# Add to ~/.bashrc
source <(myapp completion bash)
```

**Option 2: Save to file**
```bash
myapp completion bash > ~/.myapp-completion.bash
echo "source ~/.myapp-completion.bash" >> ~/.bashrc
```

### Zsh

**Option 1: Source directly in `.zshrc`**
```bash
# Add to ~/.zshrc
source <(myapp completion zsh)
```

**Option 2: Save to fpath**
```bash
# Save to a directory in your fpath
myapp completion zsh > ~/.zsh/completions/_myapp

# Ensure the directory is in your fpath (add to ~/.zshrc if needed)
fpath=(~/.zsh/completions $fpath)
autoload -U compinit && compinit
```

### Fish

**Option 1: Save to completions directory**
```bash
myapp completion fish > ~/.config/fish/completions/myapp.fish
```

**Option 2: Source in `config.fish`**
```bash
# Add to ~/.config/fish/config.fish
myapp completion fish | source
```

### PowerShell

**Option 1: Add to profile**
```powershell
# Add to your PowerShell profile
myapp completion powershell | Out-String | Invoke-Expression

# Find your profile location
$PROFILE
```

**Option 2: Save and dot-source**
```powershell
myapp completion powershell > ~/myapp-completion.ps1

# Add to your profile
. ~/myapp-completion.ps1
```

## How It Works

The completion scripts integrate with the [`[suggest]` directive](suggest.md) to provide intelligent completions:

1. When you press ++tab++, the shell calls your app with `[suggest]` and the current command line
2. Your app parses the input and returns relevant suggestions
3. The shell displays the suggestions

### What Gets Completed

- **Commands**: All visible commands and subcommands
- **Options**: Available options for the current command
- **Enum values**: Automatically suggested for enum parameters
- **Allowed values**: Values defined via `[AllowedValues]` attribute
- **Context-aware**: Suggestions respect arity, parse state, and validation rules

## Advanced Configuration

### Custom App Name

If your executable name differs from the default, configure it in `AppSettings`:

<!-- snippet: shell_completions_custom_app_name -->
<a id='snippet-shell_completions_custom_app_name'></a>
```cs
class ProgramWithCustomName
{
    static int Main(string[] args)
    {
        return new AppRunner<MyApp>(new AppSettings 
        { 
            Execution = { UsageAppName = "my-custom-name" }
        })
        .UseDefaultMiddleware()
        .Run(args);
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Completions/Shell_Completions.cs#L32-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-shell_completions_custom_app_name' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The completion scripts will use the configured app name.

### Conditional Inclusion

You can conditionally include the completion command:

<!-- snippet: shell_completions_conditional -->
<a id='snippet-shell_completions_conditional'></a>
```cs
public class MyAppWithConditionalCompletion
{
    [Subcommand]
    public CompletionCommand? Completion { get; set; } 
#if INCLUDE_COMPLETIONS
        = new();
#endif
    
    // your commands...
    public void Deploy(string environment) { }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Completions/Shell_Completions.cs#L47-L59' title='Snippet source file'>snippet source</a> | <a href='#snippet-shell_completions_conditional' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Testing Completions

Test your completions work correctly:

```bash
# Test command suggestions
myapp [suggest]

# Test option suggestions for a command
myapp [suggest] deploy

# Test value suggestions for an option
myapp [suggest] deploy --environment

# Test filtering
myapp [suggest] deploy --environment prod
```

See the [[suggest] directive documentation](suggest.md) for more details on testing.

## Distribution

### Include Installation Instructions

Add installation instructions to your README or documentation:

````markdown
## Shell Completion

Enable tab completion by running:

```bash
# Bash
echo 'source <(myapp completion bash)' >> ~/.bashrc

# Zsh  
echo 'source <(myapp completion zsh)' >> ~/.zshrc

# Fish
myapp completion fish > ~/.config/fish/completions/myapp.fish

# PowerShell
myapp completion powershell | Out-String | Invoke-Expression
```
````

### Package Managers

For applications distributed via package managers, you can install completion scripts during package installation:

**Homebrew formula example:**
```ruby
def install
  bin.install "myapp"
  
  # Install completions
  bash_completion.install "completions/myapp.bash"
  zsh_completion.install "completions/_myapp"
  fish_completion.install "completions/myapp.fish"
end
```

Generate the completion files during your build:
```bash
myapp completion bash > completions/myapp.bash
myapp completion zsh > completions/_myapp
myapp completion fish > completions/myapp.fish
```

## Examples

### Basic App

<!-- snippet: shell_completions_calculator -->
<a id='snippet-shell_completions_calculator'></a>
```cs
public class Calculator
{
    [Subcommand]
    public CompletionCommand Completion { get; set; } = new();

    public void Add(int x, int y) => Console.WriteLine(x + y);
    public void Subtract(int x, int y) => Console.WriteLine(x - y);
}

class CalculatorProgram
{
    static int Main(string[] args) =>
        new AppRunner<Calculator>()
            .UseDefaultMiddleware()
            .Run(args);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Completions/Shell_Completions.cs#L61-L78' title='Snippet source file'>snippet source</a> | <a href='#snippet-shell_completions_calculator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Users get completions for commands:
```bash
calc [tab]       # Shows: Add, Completion, Subtract
calc Add [tab]   # Shows: --x, --y
```

### With Enums

<!-- snippet: shell_completions_with_enums -->
<a id='snippet-shell_completions_with_enums'></a>
```cs
public enum Environment { Dev, Staging, Production }

public class DeployApp
{
    [Subcommand]
    public CompletionCommand Completion { get; set; } = new();

    public void Deploy(
        Environment environment,
        [Option] bool dryRun = false)
    {
        // deploy logic
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Completions/Shell_Completions.cs#L80-L95' title='Snippet source file'>snippet source</a> | <a href='#snippet-shell_completions_with_enums' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Users get enum completions:
```bash
deploy [tab]                    # Shows: --environment, --dryRun
deploy --environment [tab]      # Shows: Dev, Staging, Production
deploy --environment Pro[tab]   # Completes to: Production
```

## Troubleshooting

### Completions Not Working

1. **Verify [suggest] directive is enabled**
   ```bash
   myapp [suggest]  # Should return suggestions
   ```

2. **Check shell integration**
   - Bash: `complete -p myapp` should show the completion function
   - Zsh: `which _myapp` should show the completion function
   - Fish: Check `~/.config/fish/completions/myapp.fish` exists

3. **Reload shell configuration**
   ```bash
   # Bash
   source ~/.bashrc
   
   # Zsh
   source ~/.zshrc
   
   # Fish
   source ~/.config/fish/config.fish
   ```

### No Suggestions Appearing

- Ensure `UseDefaultMiddleware()` is called (includes suggest directive)
- Or explicitly enable: `appRunner.UseSuggestDirective()`
- Check for parse errors: `myapp [suggest] <your-command> 2>&1`

## See Also

- [[suggest] Directive](suggest.md) - How suggestions are generated
- [Directives](../Extensibility/directives.md) - Understanding directives
- [Testing Shell Completions](../TestTools/overview.md) - Test your completion scenarios
