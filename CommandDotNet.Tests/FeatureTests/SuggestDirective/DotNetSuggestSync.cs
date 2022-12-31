using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CommandDotNet.Tests.FeatureTests.SuggestDirective;

public class DotNetSuggestSync
{
    private const string RepoRoot = "https://raw.githubusercontent.com/dotnet/command-line-api/main/src";

    [Fact(Skip = "unskip to run")]
    public async Task Sync()
    {
        var client = new HttpClient();
        await SyncFile(client, "DotnetProfileDirectory.cs");
        await SyncFile(client, "FileSuggestionRegistration.cs", 
            f => f.Replace(" : ISuggestionRegistration", ""));
        await SyncFile(client, "RegistrationPair.cs");
    }

    private static async Task SyncFile(HttpClient client, string fileName, Func<string, string>? alter = null)
    {
        var source = $"https://raw.githubusercontent.com/dotnet/command-line-api/main/src/System.CommandLine.Suggest/{fileName}";
        var fileContent = await client.GetStringAsync(source);
        if (alter is not null)
        {
            fileContent = alter(fileContent);
        }

        fileContent = fileContent.Replace("namespace System.CommandLine.Suggest", 
            @"#pragma warning disable CS8600
#pragma warning disable CS8603
#pragma warning disable CS8625
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable CheckNamespace

namespace System.CommandLine.Suggest");
        
        fileContent = $@"// copied from: {source}
// via: {nameof(DotNetSuggestSync)} test class

{fileContent}";
        
        await File.WriteAllTextAsync($"../../../../CommandDotNet/DotNetSuggest/System.CommandLine.Suggest/{fileName}", fileContent);
    }
}