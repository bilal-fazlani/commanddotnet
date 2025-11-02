using JetBrains.Annotations;

namespace CommandDotNet.Completions;

/// <summary>
/// Contains shell completion script templates for bash, zsh, fish, and PowerShell.
/// Each script uses the [suggest] directive to get context-aware completions.
/// </summary>
[PublicAPI]
public static class ShellCompletionScripts
{
    public static string GetBashScript(string appName)
    {
        return $@"_{appName}_completions()
{{
    local cur prev words cword
    _init_completion || return
    local suggestions=$({appName} [suggest] ""${{COMP_WORDS[@]:[email protected]}}"" 2>/dev/null)
    if [ -n ""$suggestions"" ]; then
        COMPREPLY=( $(compgen -W ""$suggestions"" -- ""$cur"") )
    fi
}}

complete -F _{appName}_completions {appName}
";
    }

    public static string GetZshScript(string appName)
    {
        return $@"#compdef {appName}

_{appName}_completions()
{{
    local -a suggestions
    local -a response
    response=(${{(f)""$({appName} [suggest] ${{words[@]}} 2>/dev/null)""}})
    if [ ${{#response[@]}} -gt 0 ]; then
        suggestions=(${{response[@]}})
        _describe '{appName} commands' suggestions
    fi
}}

compdef _{appName}_completions {appName}
";
    }

    public static string GetFishScript(string appName)
    {
        return $@"function __{appName}_completions
    set -l tokens (commandline -opc)
    {appName} [suggest] $tokens 2>/dev/null
end

complete -c {appName} -f -a ""(__{appName}_completions)""
";
    }

    public static string GetPowerShellScript(string appName)
    {
        return $@"Register-ArgumentCompleter -Native -CommandName '{appName}' -ScriptBlock {{
    param($wordToComplete, $commandAst, $cursorPosition)
    $tokens = $commandAst.ToString() -split '\s+'
    $suggestions = & '{appName}' '[suggest]' @tokens 2>$null
    if ($suggestions) {{
        $suggestions | ForEach-Object {{
            [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
        }}
    }}
}}
";
    }
}
