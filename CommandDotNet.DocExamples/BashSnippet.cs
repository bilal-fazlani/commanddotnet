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

        public BashSnippet(string name, AppRunner runner, string appName, string args, int exitCode, string output)
        {
            Name = name;
            Runner = runner;
            AppName = appName;
            Args = args;
            ExitCode = exitCode;
            Output = string.Format(output, appName, args);
            FileText = $@"// begin-snippet: {name}
~
$ {appName} {args}
{Output}
// end-snippet";
        }
    }
}