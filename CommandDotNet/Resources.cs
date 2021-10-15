using System.Collections.Generic;

namespace CommandDotNet
{
    public class Resources
    {
        public static Resources Instance = new Resources();

        public virtual string Error_File_not_found(string fullPath) => $"File not found: {fullPath}";

        public virtual string Error_Argument_model_is_invalid(string modelName) => $"'{modelName}' is invalid";

        public virtual IEnumerable<string> Error_DataAnnotation_phrases_to_replace_with_argument_name(string memberName)
        {
            yield return $"The {memberName} field";
            yield return $"The field {memberName}";
            yield return $"The {memberName} property";
            yield return $"The property {memberName}";
        }

        public virtual string Help_Allowed_values => "Allowed values"; 
        public virtual string Help_arg => "arg";
        public virtual string Help_argument_lc => "argument";
        public virtual string Help_Arguments => "Arguments";
        public virtual string Help_arguments_lc => "arguments";
        public virtual string Help_command_lc => "command";
        public virtual string Help_Commands => "Commands";
        public virtual string Help_for_more_information_about_a_command => "for more information about a command";
        public virtual string Help_Multiple => "Multiple";
        public virtual string Help_Options => "Options";
        public virtual string Help_Options_also_available_for_subcommands => "Options also available for subcommands";
        public virtual string Help_options_lc => "options";
        public virtual string Help_Usage => "Usage";
        public virtual string Help_usage_lc => "usage";
        public virtual string Help_Use => "Use";

        public virtual string Type_Boolean => "Boolean";
        public virtual string Type_Character => "Character";
        public virtual string Type_Decimal => "Decimal";
        public virtual string Type_Double => "Double";
        public virtual string Type_Number => "Number";
        public virtual string Type_Text => "Text";

        public virtual string Command_help => "help";
        public virtual string Command_help_description => "Show help information";
        public virtual string Command_version => "version";
        public virtual string Command_version_description => "Show version information";

        public virtual string Input_piped_lc => "piped";
        public virtual string Input_prompt_lc => "prompt";

        public virtual string ValueSource_EnvVar => "EnvVar";
        public virtual string ValueSource_AppSetting => "AppSetting";
    }
}
