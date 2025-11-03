using CommandDotNet.Completions;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Completions;

public class CompletionCommandTests
{
    public CompletionCommandTests(ITestOutputHelper output) => Ambient.Output = output;

    [Fact]
    public void Help_ShowsCompletionCommandWithSubcommands() =>
        new AppRunner<AppWithCompletion>()
            .Verify(new Scenario
            {
                When = { Args = "completion -h" },
                Then = 
                { 
                    Output = @"Generate shell completion scripts

Usage: testhost.dll completion [command]

Commands:

  Bash        Generate shell completion script for bash
  Fish        Generate shell completion script for fish
  Powershell  Generate shell completion script for PowerShell
  Zsh         Generate shell completion script for zsh

Use ""testhost.dll completion [command] --help"" for more information about a command."
                }
            });

    [Fact]
    public void Bash_OutputsScript() =>
        new AppRunner<AppWithCompletion>(new AppSettings { Execution = { UsageAppName = "myapp" } })
            .Verify(new Scenario
            {
                When = { Args = "completion Bash" },
                Then =
                {
                    Output = @"_myapp_completions()
{
    local cur prev words cword
    _init_completion || return
    local suggestions=$(myapp [suggest] ""${COMP_WORDS[@]:[email protected]}"" 2>/dev/null)
    if [ -n ""$suggestions"" ]; then
        COMPREPLY=( $(compgen -W ""$suggestions"" -- ""$cur"") )
    fi
}

complete -F _myapp_completions myapp"
                }
            });

    [Fact]
    public void Zsh_OutputsScript() =>
        new AppRunner<AppWithCompletion>(new AppSettings { Execution = { UsageAppName = "myapp" } })
            .Verify(new Scenario
            {
                When = { Args = "completion Zsh" },
                Then =
                {
                    Output = @"#compdef myapp

_myapp_completions()
{
    local -a suggestions
    local -a response
    response=(${(f)""$(myapp [suggest] ${words[@]} 2>/dev/null)""})
    if [ ${#response[@]} -gt 0 ]; then
        suggestions=(${response[@]})
        _describe 'myapp commands' suggestions
    fi
}

compdef _myapp_completions myapp"
                }
            });

    [Fact]
    public void Fish_OutputsScript() =>
        new AppRunner<AppWithCompletion>(new AppSettings { Execution = { UsageAppName = "myapp" } })
            .Verify(new Scenario
            {
                When = { Args = "completion Fish" },
                Then =
                {
                    Output = @"function __myapp_completions
    set -l tokens (commandline -opc)
    myapp [suggest] $tokens 2>/dev/null
end

complete -c myapp -f -a ""(__myapp_completions)"""
                }
            });

    [Fact]
    public void PowerShell_OutputsScript() =>
        new AppRunner<AppWithCompletion>(new AppSettings { Execution = { UsageAppName = "myapp" } })
            .Verify(new Scenario
            {
                When = { Args = "completion Powershell" },
                Then =
                {
                    Output = @"Register-ArgumentCompleter -Native -CommandName 'myapp' -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)
    $tokens = $commandAst.ToString() -split '\s+'
    $suggestions = & 'myapp' '[suggest]' @tokens 2>$null
    if ($suggestions) {
        $suggestions | ForEach-Object {
            [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
        }
    }
}"
                }
            });

    [Fact]
    public void UsesAppNameFromExecutionSettings() =>
        new AppRunner<AppWithCompletion>(new AppSettings { Execution = { UsageAppName = "customname" } })
            .Verify(new Scenario
            {
                When = { Args = "completion Bash" },
                Then =
                {
                    OutputContainsTexts = 
                    { 
                        "_customname_completions()",
                        "complete -F _customname_completions customname"
                    }
                }
            });

    [Fact]
    public void Help_Bash_ShowsDescriptionAndInstallation() =>
        new AppRunner<AppWithCompletion>()
            .Verify(new Scenario
            {
                When = { Args = "completion Bash -h" },
                Then = 
                { 
                    Output = @"Generate shell completion script for bash

Usage: testhost.dll completion Bash

Installation:
  Add to ~/.bashrc:
    source <(testhost.dll completion bash)
  
  Or save to a file:
    testhost.dll completion bash > ~/.testhost.dll-completion.bash
    source ~/.testhost.dll-completion.bash"
                }
            });

    [Fact]
    public void Help_Zsh_ShowsDescriptionAndInstallation() =>
        new AppRunner<AppWithCompletion>()
            .Verify(new Scenario
            {
                When = { Args = "completion Zsh -h" },
                Then = 
                { 
                    Output = @"Generate shell completion script for zsh

Usage: testhost.dll completion Zsh

Installation:
  Add to ~/.zshrc:
    source <(testhost.dll completion zsh)
  
  Or save to a file in your fpath:
    testhost.dll completion zsh > ~/.zsh/completions/_testhost.dll
    # Make sure ~/.zsh/completions is in your fpath"
                }
            });

    [Fact]
    public void Help_Fish_ShowsDescriptionAndInstallation() =>
        new AppRunner<AppWithCompletion>()
            .Verify(new Scenario
            {
                When = { Args = "completion Fish -h" },
                Then = 
                { 
                    Output = @"Generate shell completion script for fish

Usage: testhost.dll completion Fish

Installation:
  Save to completions directory:
    testhost.dll completion fish > ~/.config/fish/completions/testhost.dll.fish
  
  Or add to config.fish:
    testhost.dll completion fish | source"
                }
            });

    [Fact]
    public void Help_PowerShell_ShowsDescriptionAndInstallation() =>
        new AppRunner<AppWithCompletion>()
            .Verify(new Scenario
            {
                When = { Args = "completion Powershell -h" },
                Then = 
                { 
                    Output = @"Generate shell completion script for PowerShell

Usage: testhost.dll completion Powershell

Installation:
  Add to your PowerShell profile:
    testhost.dll completion powershell | Out-String | Invoke-Expression
  
  Or save and dot-source:
    testhost.dll completion powershell > ~/testhost.dll-completion.ps1
    . ~/testhost.dll-completion.ps1
  
  Find your profile location: $PROFILE"
                }
            });

    private class AppWithCompletion
    {
        [Subcommand]
        public CompletionCommand Completion { get; set; } = new();

        public void SomeCommand()
        {
        }
    }
}
