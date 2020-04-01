using System;

namespace CommandDotNet.Directives.Parse
{
    [Obsolete("Moved to namespace Diagnostics.Parse")]
    public static class ParseReporter
    {
        [Obsolete("User Diagnostics.Parse.ParseReporter.Report instead")]
        public static void Report(CommandContext commandContext, 
            Action<string> writeln, Indent indent = null)
        {
            Diagnostics.Parse.ParseReporter.Report(commandContext, writeln, indent);
        }
    }
}