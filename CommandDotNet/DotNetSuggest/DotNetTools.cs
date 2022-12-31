using System;
using System.CommandLine.Suggest;
using System.IO;
using CommandDotNet.Builders;

namespace CommandDotNet.DotNetSuggest;

public static class DotNetTools
{
    // see https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Suggest/GlobalToolsSuggestionRegistration.cs
    // for example
    public static string? GlobalToolDirectory =>
        DotnetProfileDirectory.TryGet(out string directory)
            ? Path.Combine(directory, "tools")
            : null;
}