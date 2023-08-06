using System;
using System.CommandLine.Suggest;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.DotnetSuggest;

namespace CommandDotNet.Example.Commands;

[Command(Description = "(Un)registers this sample app with `dotnet suggest` to provide auto complete")]
public class DotnetSuggest
{
    public void Register(IConsole console, IEnvironment environment)
    {
        var registered = DotnetTools.EnsureRegisteredWithDotnetSuggest(environment, out var results, console);
        var appInfo = AppInfo.Instance;
        console.WriteLine(registered
            ? results is null
                ? appInfo.IsGlobalTool
                    ? "Already registered. Global tools are registered by default."
                    : "Already registered"
                : $"Succeeded:{Environment.NewLine}{results.ToString(new Indent(depth: 1), skipOutputs: true)}"
            : $"Failed:{Environment.NewLine}{results!.ToString(new Indent(depth: 1), skipOutputs: true)}");
    }

    public async Task Unregister(IConsole console)
    {
        if (AppInfo.Instance.IsGlobalTool)
        {
            console.WriteLine("This is a global tool. Global tools are registered by default and cannot be unregistered.");
        }
        
        var path = new FileSuggestionRegistration().RegistrationConfigurationFilePath;
        var lines = await File.ReadAllLinesAsync(path);
        var newLines = lines.Where(l => !l.StartsWith(AppInfo.Instance.FilePath)).ToArray();

        if (lines.Length == newLines.Length)
        {
            console.WriteLine("Not registered with dotnet-suggest");
        }
        else
        {
            await File.WriteAllLinesAsync(path, newLines);
            console.WriteLine("Unregistered with dotnet-suggest");
        }
    }
}