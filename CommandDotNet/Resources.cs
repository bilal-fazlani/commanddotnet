using System.Diagnostics.CodeAnalysis;

namespace CommandDotNet
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class Resources
    {
        public static Resources A = new();

        public virtual string Common_default_lc => "default";
        public virtual string Common_from_lc => "from";
        public virtual string Common_key_lc => "key";
        public virtual string Common_value_lc => "value";
        public virtual string Common_source_lc => "source";
        public virtual string Common_command_lc => "command";
        public virtual string Common_argument_lc => "argument";
        public virtual string Common_option_lc => "option";
        public virtual string Common_Flag => "flag";
        public virtual string Common_commands_lc => "commands";
        public virtual string Common_arguments_lc => "arguments";
        public virtual string Common_options_lc => "options";

        public virtual string Error_File_not_found(string fullPath) => $"File not found: {fullPath}";
    }
}
