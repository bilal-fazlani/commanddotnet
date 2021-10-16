// ReSharper disable CheckNamespace

using System.Diagnostics;

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

        public virtual string Parse_Raw_command_line(string commandLine) => 
            $"raw command line:{commandLine}";
        public virtual string Parse_root_lc => "root";
        
        public virtual string Debugger_debug_lc => "debug";
        public virtual string Debugger_Attach_debugger(Process process) => 
            $"Attach your debugger to process {process.Id} ({process.ProcessName}).";
    }
}