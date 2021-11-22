using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.DocExamples
{
    public static class SnippetExtensions
    {
        public static void VerifySnippet(this BashSnippet snippet)
        {
            var result = snippet.Runner
                .Configure(c => c.AppSettings.Help.UsageAppName = snippet.AppName)
                .Verify(new Scenario
            {
                When = { Args = snippet.Args },
                Then =
                {
                    ExitCode = snippet.ExitCode,
                    Output = snippet.Output
                }
            });
        }
    }
}