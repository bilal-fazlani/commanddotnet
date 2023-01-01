using System;
using System.CommandLine.Suggest;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandDotNet.Builders;

namespace CommandDotNet.DotnetSuggest;

public static class DotnetTools
{
    // for example, see
    // https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Suggest/GlobalToolsSuggestionRegistration.cs
    public static string? GlobalToolDirectory =>
        DotnetProfileDirectory.TryGet(out string directory)
            ? Path.Combine(directory, "tools")
            : null;
    
    public static bool EnsureRegisteredWithDotnetSuggest(IEnvironment environment,
        [NotNullWhen(false)] out ExternalCommand? results, IConsole? console = null)
    {
        // see System.CommandLine's CommandLineBuilderExtensions.RegisterWithDotnetSuggest for reference

        var appInfo = AppInfo.Instance;
        results = null;

        if (appInfo.IsGlobalTool)
        {
            // already registered with DotnetSuggest see System.CommandLine's GlobalToolsSuggestionRegistration
            return true;
        }

        var reg = new FileSuggestionRegistration().FindRegistration(new FileInfo(appInfo.FilePath));
        if (reg is not null)
        {
            // already registered with DotnetSuggest
            return true;
        }

        EnsurePathEnvVarIsSetForOsShellScript(environment, appInfo);

        results = ExternalCommand.Run("dotnet-suggest",
                $"register --command-path \"{appInfo.FilePath}\" " +
                $"--suggestion-command \"{Path.GetFileNameWithoutExtension(appInfo.FileName)}\"",
                console);
        return results.Succeeded;
    }

    private static void EnsurePathEnvVarIsSetForOsShellScript(IEnvironment environment, AppInfo appInfo)
    {
        var path = environment.GetEnvironmentVariable("PATH") ?? "";
        var directoryName = Path.GetDirectoryName(appInfo.FilePath)!;
        if (!path.Contains(directoryName))
        {
            path += path.IsNullOrWhitespace() 
                ? appInfo.FilePath 
                : Path.PathSeparator + appInfo.FilePath;
            environment.SetEnvironmentVariables("PATH", path, EnvironmentVariableTarget.User);
        }
    }
}