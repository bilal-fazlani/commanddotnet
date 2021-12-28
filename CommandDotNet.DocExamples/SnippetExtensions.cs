using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.DocExamples
{
    public static class SnippetExtensions
    {
        public static IEnumerable<BashSnippet> GetSnippets(this Type type)
        {
            return type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(BashSnippet))
                .Select(field =>
                {
                    var snippet = (BashSnippet)field.GetValue(null)!;
                    snippet.Member = field;
                    return snippet;
                });
        }

        public static void VerifySnippet(this BashSnippet snippet)
        {
            var result = snippet.Runner
                .Configure(c => c.AppSettings.Help.UsageAppName = snippet.AppName)
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = snippet.Args,
                        PipedInput = snippet.PipedInput,
                        OnPrompt = snippet.PromptAnswers is null ? null : new PromptResponder(snippet.PromptAnswers)
                    },
                    Then =
                    {
                        ExitCode = snippet.ExitCode,
                        Output = snippet.Output
                    }
                });
        }
    }
}