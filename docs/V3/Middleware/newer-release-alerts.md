# Newer release alerts

This middleware will check a url for the latest version of your app and alert the user if a new release is found.

## Github

use `appRunner.UseNewerReleaseAlertOnGitHub(organizationName, repositoryName, ...)` if your app is published as a GitHub release.

This will get the version from the latest release. 

!!! warning
    This is not suitable for a mono-repo where multiple products may be released. 

### Parameters

* `getVersionFromReleaseName`: use if your release names do not follow a standard v1.0.0-prefix 
* `overrideHttpRequestCallback`: append headers, auth info, etc
* `skipCommand`: do not run for commands that pipe output or skip if the command is not the root command.

## Generic

use `appRunner.UseNewerReleaseAlert(...)` to check any url.  `UseNewerReleaseAlertOnGitHub` wraps this method and you can use it as an example.

### Parameters

* `latestReleaseUrl`: the url for metadata about the latest release
* `parseSemanticVersionFromResponseBodyCallback`: callback to get the semantic version for response from the `latestReleaseUrl`
* `postfixAlertMessageCallback`: the results of this callback will be post-fixed to the alert message. e.g. download link.
* `overrideHttpRequestCallback`: append headers, auth info, etc
* `skipCommand`: do not run for commands that pipe output or skip if the command is not the root command.

nuget package: [CommandDotNet.NewerReleasesAlerts](https://www.nuget.org/packages/CommandDotNet.NewerReleasesAlerts)

Inspired by https://github.com/solidify/jira-azuredevops-migrator