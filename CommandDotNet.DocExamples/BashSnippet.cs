using System.Reflection;
using System.Runtime.CompilerServices;
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

        public string Output { get; }
        public string FileText { get; }

        public MemberInfo? Member { get; set; }
        public string? MemberName => Member is null ? null : $"{Member.DeclaringType!.Name}.{Member.Name}";

        public BashSnippet(string name, AppRunner runner, string appName, string args, int exitCode, string output, bool argsOnlySnippet = false)
        {
            const string beginSnippet = "begin-snippet";
            Name = name;
            Runner = runner;
            AppName = appName;
            Args = args;
            ExitCode = exitCode;
            Output = string.Format(output, appName, args);
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