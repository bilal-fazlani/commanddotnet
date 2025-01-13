﻿using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CommandDotNet.NewerReleasesAlerts;

public static class GitHubMiddleware
{
    /// <summary>
    /// This middleware will check if a newer release is available in GitHub releases
    /// and output an alert with a download link if a new release exists.
    /// </summary>
    /// <param name="appRunner">the <see cref="AppRunner"/> to run the middleware</param>
    /// <param name="organizationName">The name of the organization in GitHub</param>
    /// <param name="repositoryName">The name of the repository in GitHub</param>
    /// <param name="getVersionFromReleaseName">Provide this in cases where the release name does not follow a standard 'v1.0.0-prefix' naming convention.</param>
    /// <param name="overrideHttpRequestCallback">use this callback to append headers and auth info for tests. Also useful for mocking requests for unit tests.</param>
    /// <param name="skipCommand">
    /// Use this to skip the alert for commands where the alert would result in bad output.
    /// i.e. Command output that could be piped to another command.
    /// </param>
    public static AppRunner UseNewerReleaseAlertOnGitHub(this AppRunner appRunner,
        string organizationName, string repositoryName,
        Func<string?, string?>? getVersionFromReleaseName = null,
        OverrideHttpRequestCallback? overrideHttpRequestCallback = null,
        Predicate<Command>? skipCommand = null)
    {
        getVersionFromReleaseName ??= name => name;

        var latestReleaseUrl = BuildLatestReleaseUrl(organizationName, repositoryName);
        return appRunner.UseNewerReleaseAlert(
            latestReleaseUrl, 
            response => getVersionFromReleaseName(GetNameFromReleaseBody(response)),
            version => $"Download from {BuildDownloadUrl(organizationName, repositoryName, version)}",
            overrideHttpRequestCallback, skipCommand);
    }

    private static string? GetNameFromReleaseBody(string? response)
    {
        if (response is null)
        {
            return null;
        }

        var o = JsonSerializer.Deserialize<JsonObject>(response);
        return o?.TryGetPropertyValue("tag_name", out var node) ?? false 
            ? node.Deserialize<string>()?.Replace("v", "") 
            : null;
    }
        
    private static string BuildLatestReleaseUrl(string organizationName, string repoName) => $"https://api.github.com/repos/{organizationName}/{repoName}/releases/latest";
    private static string BuildDownloadUrl(string organizationName, string repoName, string version) => $"https://github.com/{organizationName}/{repoName}/releases/tag/{version}";
}