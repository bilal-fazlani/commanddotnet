namespace CommandDotNet
{
    public class HelpText
    {
        public static HelpText Instance = new HelpText();

        public virtual string Allowed_values => "Allowed values";
        public virtual string arg => "arg";
        public virtual string argument_lc => "argument";
        public virtual string Arguments => "Arguments";
        public virtual string arguments_lc => "arguments";
        public virtual string Boolean => "Boolean";
        public virtual string Character => "Character";
        public virtual string command_lc => "command";
        public virtual string Commands => "Commands";
        public virtual string Decimal => "Decimal";
        public virtual string Double => "Double";
        public virtual string for_more_information_about_a_command => "for more information about a command";
        public virtual string help_lc => "help";
        public virtual string Multiple => "Multiple";
        public virtual string Number => "Number";
        public virtual string Options => "Options";
        public virtual string Options_also_available_for_subcommands => "Options also available for subcommands";
        public virtual string options_lc => "options";
        public virtual string piped_lc => "piped";
        public virtual string prompt_lc => "prompt";
        public virtual string Show_help_information => "Show help information";
        public virtual string Show_version_information => "Show version information";
        public virtual string Text => "Text";
        public virtual string Usage => "Usage";
        public virtual string usage_lc => "usage";
        public virtual string Use => "Use";
        public virtual string version_lc => "version";
    }
}
