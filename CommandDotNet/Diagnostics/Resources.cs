// ReSharper disable CheckNamespace

using static System.Environment;

namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Command_version => "version";
        public virtual string Command_version_description => "Show version information";

        public virtual string CommandLogger_Original_input => "Original input";
        public virtual string CommandLogger_Tool_version => "Tool version";
        public virtual string CommandLogger_DotNet_version => ".Net version";
        public virtual string CommandLogger_OS_version => "OS version";
        public virtual string CommandLogger_Machine => "Machine";
        public virtual string CommandLogger_Username => "Username";

        public virtual string Exceptions_Data => "Data";
        public virtual string Exceptions_Properties => "Properties";
        public virtual string Exceptions_StackTrace => "StackTrace";
        public virtual string Exceptions_StackTrace_at => "at";

        public virtual string ParseReport_Raw_command_line(string commandLine) => 
            $"raw command line:{commandLine}";

        public virtual string ParseReport_root_lc => "root";

        public virtual string ParseDirective_Help_was_requested =>
            "Help requested. Only token transformations are available.";

        public virtual string ParseDirective_Unable_to_map_tokens_to_arguments =>
            "Unable to map tokens to arguments. Falling back to token transformations.";

        public virtual string ParseDirective_Usage(string includeTransformationsArgName, string includeRawCommandLineArgName) =>
            $"Parse usage: [parse:{includeTransformationsArgName}:{includeRawCommandLineArgName}] to include token transformations.{NewLine}" +
            $" '{includeTransformationsArgName}' to include token transformations.{NewLine}" +
            $" '{includeRawCommandLineArgName}' to include command line as passed to this process.";

        public virtual string ParseDirective_parse_lc => "parse";
        public virtual string ParseDirective_token_transformations_lc => "token transformations";
        public virtual string ParseDirective_from_shell_lc => "from shell";
        public virtual string ParseDirective_after(string transformationName) => $"after: {transformationName}";
        public virtual string ParseDirective_after_no_changes(string transformationName) => $"after: {transformationName} (no changes)";

        public virtual string Debugger_debug_lc => "debug";

        public virtual string Debugger_Attach_debugger(string processId, string processName) =>
            $"Attach your debugger to process {processId} ({processName}).";

        public virtual string Time_time => "time";
    }
}