using System.Threading.Tasks;
using CommandDotNet.Builders;
using JetBrains.Annotations;

namespace CommandDotNet.Completions;

/// <summary>
/// A command class that apps can include to provide shell completion script generation.
/// Add as a subcommand property in your root command class.
/// The executable app name is determined from <see cref="AppSettings.Execution"/>.
/// </summary>
/// <example>
/// <code>
/// public class MyApp
/// {
///     [Subcommand]
///     public CompletionCommand Completion { get; set; } = new();
///     
///     // your other commands...
/// }
/// 
/// new AppRunner&lt;MyApp&gt;().Run(args);
/// 
/// // Users can then run: myapp completion bash
/// </code>
/// </example>
[PublicAPI]
[Command("completion", Description = "Generate shell completion scripts")]
public class CompletionCommand
{
    private string? _appName;

    public Task<int> Intercept(InterceptorExecutionDelegate next, CommandContext commandContext)
    {
        _appName = AppInfo.GetExecutableAppName(commandContext.AppConfig.AppSettings.Execution);
        return next();
    }

    [Command(Description = "Generate shell completion script for bash",
             ExtendedHelpText = @"Installation:
  Add to ~/.bashrc:
    source <(%AppName% completion bash)
  
  Or save to a file:
    %AppName% completion bash > ~/.%AppName%-completion.bash
    source ~/.%AppName%-completion.bash")]
    public void Bash(IConsole console) => 
        console.Out.WriteLine(ShellCompletionScripts.GetBashScript(_appName!));

    [Command(Description = "Generate shell completion script for zsh",
             ExtendedHelpText = @"Installation:
  Add to ~/.zshrc:
    source <(%AppName% completion zsh)
  
  Or save to a file in your fpath:
    %AppName% completion zsh > ~/.zsh/completions/_%AppName%
    # Make sure ~/.zsh/completions is in your fpath")]
    public void Zsh(IConsole console) => 
        console.Out.WriteLine(ShellCompletionScripts.GetZshScript(_appName!));

    [Command(Description = "Generate shell completion script for fish",
             ExtendedHelpText = @"Installation:
  Save to completions directory:
    %AppName% completion fish > ~/.config/fish/completions/%AppName%.fish
  
  Or add to config.fish:
    %AppName% completion fish | source")]
    public void Fish(IConsole console) => 
        console.Out.WriteLine(ShellCompletionScripts.GetFishScript(_appName!));

    [Command(Description = "Generate shell completion script for PowerShell",
             ExtendedHelpText = @"Installation:
  Add to your PowerShell profile:
    %AppName% completion powershell | Out-String | Invoke-Expression
  
  Or save and dot-source:
    %AppName% completion powershell > ~/%AppName%-completion.ps1
    . ~/%AppName%-completion.ps1
  
  Find your profile location: $PROFILE")]
    public void Powershell(IConsole console) => 
        console.Out.WriteLine(ShellCompletionScripts.GetPowerShellScript(_appName!));
}
