using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.DocExamples;

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
        // Expand $* bash syntax to actual piped input values
        // When $* is used with an option, each value needs its own option flag
        var args = snippet.Args;
        IEnumerable<string>? pipedInput = snippet.PipedInput;
        
        if (snippet.PipedInput != null && args.Contains("$*"))
        {
            // Check if $* is preceded by an option flag (e.g., "--notify $*")
            var parts = args.Split(new[] { "$*" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var beforeExpansion = parts[0].TrimEnd();
                var afterExpansion = parts[1];
                
                // Extract the option name (last token before $*)
                var tokens = beforeExpansion.Split(' ');
                var lastToken = tokens[tokens.Length - 1];
                
                if (lastToken.StartsWith("-"))
                {
                    // Repeat the option for each piped value: "--notify a1 --notify b1 --notify c3"
                    var repeated = string.Join(" ", snippet.PipedInput.Select(v => $"{lastToken} {v}"));
                    args = string.Join(" ", tokens.Take(tokens.Length - 1)) + " " + repeated + afterExpansion;
                }
                else
                {
                    // Simple expansion without option repetition
                    args = args.Replace("$*", string.Join(" ", snippet.PipedInput));
                }
            }
            else
            {
                // Simple replacement if $* appears in an unexpected location
                args = args.Replace("$*", string.Join(" ", snippet.PipedInput));
            }
            
            pipedInput = null; // $* expansion means values go to args, not stdin
        }

        var result = snippet.Runner
            .Configure(c => c.AppSettings.Execution.UsageAppName = snippet.AppName)
            .Verify(new Scenario
            {
                When =
                {
                    Args = args,
                    PipedInput = pipedInput,
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