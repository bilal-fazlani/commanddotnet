using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using Semver;

namespace CommandDotNet.NewerReleasesAlerts
{
    public static class AlertOnNewerReleaseMiddleware
    {
        public delegate string ParseSemanticVersionFromResponseBodyDelegate(string responseBody);
        public delegate string PostfixAlertMessageDelegate(string latestReleaseVersion);

        public delegate Task<string> OverrideHttpRequestCallback(HttpClient client, Uri requestUri);
        
        /// <summary>
        /// This middleware will call the <see cref="latestReleaseUrl"/> and
        /// use <see cref="parseSemanticVersionFromResponseBodyCallback"/> to parse a
        /// semantic version from the response.<br/>
        /// If the version is greater than the file version of the assembly,
        /// an alert is output to the console.<br/>
        /// See <see cref="GitHubMiddleware"/> for an example of how to use this method.
        /// </summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> to run the middleware</param>
        /// <param name="latestReleaseUrl">the url for metadata about the latest release</param>
        /// <param name="parseSemanticVersionFromResponseBodyCallback">callback to get the semantic version for response from the <see cref="latestReleaseUrl"/></param>
        /// <param name="postfixAlertMessageCallback">the results of this callback will be post-fixed to the alert message.  i.e. download link.</param>
        /// <param name="overrideHttpRequestCallback">use this callback to append headers and auth info for tests. Also useful for mocking requests for unit tests.</param>
        public static AppRunner UseNewerReleaseAlert(this AppRunner appRunner, 
            string latestReleaseUrl,
            ParseSemanticVersionFromResponseBodyDelegate parseSemanticVersionFromResponseBodyCallback,
            PostfixAlertMessageDelegate postfixAlertMessageCallback = null,
            OverrideHttpRequestCallback overrideHttpRequestCallback = null)
        {
            return appRunner.Configure(c =>
            {
                c.Services.Add(new NewerReleaseConfig
                {
                    LatestReleaseUrl = latestReleaseUrl,
                    ParseSematicVersionFromResponseBodyCallback = parseSemanticVersionFromResponseBodyCallback,
                    PostfixAlertMessageCallback = postfixAlertMessageCallback,
                    OverrideHttpRequestCallback = overrideHttpRequestCallback
                });
                c.UseMiddleware(AlertOnNewVersion, MiddlewareStages.PreTransformTokens);
            });
        }

        private class NewerReleaseConfig
        {
            public string LatestReleaseUrl;
            public ParseSemanticVersionFromResponseBodyDelegate ParseSematicVersionFromResponseBodyCallback;
            public PostfixAlertMessageDelegate PostfixAlertMessageCallback;
            public OverrideHttpRequestCallback OverrideHttpRequestCallback { get; set; }
        }

        private static Task<int> AlertOnNewVersion(CommandContext context, Func<CommandContext, Task<int>> next)
        {
            NewerReleaseConfig config = context.AppConfig.Services.Get<NewerReleaseConfig>();

            SemVersion latestReleaseVersion = null;

            var shouldAlert = TryGetCurrentVersion(context, out var currentVersion)
                              && TryGetLatestReleaseVersion(context, config, out latestReleaseVersion)
                              && currentVersion < latestReleaseVersion;
            if (shouldAlert)
            {
                var message = $"A newer release exists. Current:{currentVersion} Latest:{latestReleaseVersion}";
                if (config.PostfixAlertMessageCallback != null)
                {
                    message += $" {config.PostfixAlertMessageCallback?.Invoke(latestReleaseVersion.ToString())}";
                }
                context.Console.Out.WriteLine(message);
                context.Console.Out.WriteLine();
            }

            return next(context);
        }

        private static bool TryGetCurrentVersion(CommandContext context, out SemVersion semVersion)
        {
            var versionInfo = VersionInfo.GetVersionInfo(context);
            return SemVersion.TryParse(versionInfo.Version, out semVersion);

        }

        private static bool TryGetLatestReleaseVersion(CommandContext context, NewerReleaseConfig config, out SemVersion semVersion)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

                    var requestUri = new Uri(config.LatestReleaseUrl);

                    var response = config.OverrideHttpRequestCallback?.Invoke(httpClient, requestUri).Result
                                   ?? httpClient.GetStringAsync(requestUri).Result;

                    var version = config.ParseSematicVersionFromResponseBodyCallback(response);

                    return SemVersion.TryParse(version, out semVersion);
                }
            }
            catch(Exception e)
            {
                // don't fail operation just for this
                context.Console.Error.WriteLine($"failed to retrieve latest release info. {e.Message}");
                semVersion = null;
                return false;
            }
        }
    }
}
