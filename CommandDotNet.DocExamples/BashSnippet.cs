using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.TestTools.Prompts;
using NuGet.Frameworks;

namespace CommandDotNet.DocExamples
{
    public class BashSnippet
    {
        public string Name { get; }
        public AppRunner Runner { get; }
        public string AppName { get; }
        public string Args { get; }
        public int ExitCode { get; }
        public IAnswer[]? PromptAnswers { get; }

        public string Output { get; }
        public string FileText { get; }

        public string[]? PipedInput { get; }

        public MemberInfo? Member { get; set; }
        public string? MemberName => Member is null ? null : $"{Member.DeclaringType!.Name}.{Member.Name}";

        public BashSnippet(string name, AppRunner runner, 
            string appName, string args, int exitCode, string output, 
            bool argsOnlySnippet = false,
            (string source, string[] inputs)? pipedInput = null,
            IAnswer[]? promptAnswers = null)
        {
            const string beginSnippet = "begin-snippet";

            Name = name;
            Runner = runner;
            AppName = appName;
            Args = args;
            ExitCode = exitCode;
            PromptAnswers = promptAnswers;
            Output = string.Format(output, appName, args);

            if (pipedInput is not null)
            {
                appName = $"{pipedInput.Value.source} | {appName}";
                PipedInput = pipedInput.Value.inputs;
            }

            FileText = argsOnlySnippet 
                ? $@"// {beginSnippet}: {name}
$ {appName} {args}
// end-snippet" 
                : $@"// {beginSnippet}: {name}
$ {appName} {args}
{Output}
// end-snippet";
        }
    }
}