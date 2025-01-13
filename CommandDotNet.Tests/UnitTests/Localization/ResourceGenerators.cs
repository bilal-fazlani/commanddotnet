using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools;
using Namotion.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.UnitTests.Localization;

public class ResourceGenerators
{
    private readonly ITestOutputHelper _output;
    private static readonly string SolutionRoot = 
        new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!.Parent!.FullName;

    public ResourceGenerators(ITestOutputHelper output)
    {
        Ambient.Output = _output = output;
    }

    // possible file formats: https://docs.lokalise.com/en/collections/2909121-keys-and-files#supported-file-formats
    // resx with IStringLocalizer is problematic because the keys are case-insensitive in the format so we 
    // cannot have "arguments" and "Arguments", though we need both.

    //[Fact(Skip = "run to regenerate the ResourceProxy classes")]
    [Fact]
    public void RegenerateAll()
    {
        RegenerateProxyClasses();

        var proxies = ResourceProxyTests.ResourcesDefs
            .Select(r => r.proxy)
            .ToList();

        RegenerateSimpleJsonFiles(proxies);
        RegenerateResxFiles(proxies);
    }

    #region Add File Formats Here

    private void RegenerateResxFiles(List<ResourcesDef> proxies)
    {
        RegenerateFiles(proxies, "resx", "resx",
            (proxy, parts) => 
                ResxBuilder.Build(parts, false));
    }

    private void RegenerateSimpleJsonFiles(List<ResourcesDef> proxies)
    {
        RegenerateFiles(proxies, "simple_json", "json",
            (proxy, parts) =>
                JsonSerializer.Serialize(parts.ToDictionary(t => t.value, t => t.value)));
    }

    #endregion

    internal void RegenerateProxyClasses()
    {
        var sources = ResourceProxyTests.ResourcesDefs.Select(r => r.source).ToList();
            
        var errors = sources.SelectMany(s => s.Validate()).ToList();
        if (errors.Any())
        {
            errors.ForEach(e => _output.WriteLine(
                $"{e.error}:{Environment.NewLine}" +
                $"  {e.members.ToCsv($"{Environment.NewLine}  ")}"));
            Assert.True(false);
        }

        var header = "// this file generated by " +
                     $"{GetType().Name}.{nameof(RegenerateProxyClasses)}{Environment.NewLine}";
            
        sources.ForEach(s =>
        {
            //File.Exists(path).Should().BeTrue("class should exist: {0}", path);
            var proxyClass = s.GenerateProxyClass("ResourcesProxy", true);
            var path = Path.Combine(SolutionRoot, s.Type.Namespace!, "ResourcesProxy.cs");
            File.WriteAllText(path, $"{header}{proxyClass}");
        });
    }

    private void RegenerateFiles(List<ResourcesDef> proxies, string format, string fileExtension,
        Func<ResourcesDef, List<(string memberName, string value, string comments)>, string> buildFileText)
    {
        proxies.ForEach(p => RegenerateFiles(p, format, fileExtension, buildFileText));
    }

    private void RegenerateFiles(ResourcesDef proxy, string format, string fileExtension,
        Func<ResourcesDef, List<(string memberName, string value, string comments)>, string> buildFileText)
    {
        var localizationParts = proxy.GetLocalizationParts();

        var fileName = $"{proxy.Type.Namespace}.{fileExtension}";
        var folder = new[] { SolutionRoot, "localization_files", format, "en" }.EnsureDirectoryExists();

        File.WriteAllText(Path.Combine(folder, fileName), buildFileText(proxy, localizationParts));
    }
}

public static class ResourceGeneratorHelperExtensions
{
    private static readonly Dictionary<string, List<(string key, string value, string comments)>> PartsByProxy = new();

    public static List<(string memberName, string value, string comments)> GetLocalizationParts(this ResourcesDef proxy)
    {
        // TODO: include parameter names and xml comments, mapped to the placeholder number.
        return PartsByProxy!.GetOrAdd(proxy.Type.FullName, k => proxy.GetMembersWithDefaults()
            .Select(mwd => (mwd.member.Name, mwd.value, mwd.member.GetXmlDocsSummary()))
            .ToList());
    }

    public static string EnsureDirectoryExists(this string[] paths) => 
        EnsureDirectoryExists(new DirectoryInfo(Path.Combine(paths))).FullName;

    public static DirectoryInfo EnsureDirectoryExists(this DirectoryInfo directory)
    {
        if (!directory.Exists)
        {
            EnsureDirectoryExists(directory.Parent!);
            directory.Create();
        }

        return directory;
    }
}