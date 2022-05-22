using Microsoft.Extensions.Localization;
using System.Text;

namespace CommandDotNet.LocalizationExample.Shared
{
    /// <summary>
    /// class for CommandDotNet resource override
    /// </summary>
    public class CoreResources : CommandDotNet.Resources
    {
        private readonly IStringLocalizer<CoreResources> _Localizer;

        /// <summary>
        /// Constructor for Dependency injection
        /// </summary>
        /// <param name="localizer"></param>
        public CoreResources(IStringLocalizer<CoreResources> localizer)
        {
            _Localizer = localizer;
        }
        /// <summary>
        /// 
        /// </summary>
        public override string Common_default_lc => _Localizer.GetString("Common_default_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_from_lc => _Localizer.GetString("Common_from_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_key_lc => _Localizer.GetString("Common_key_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_value_lc => _Localizer.GetString("Common_value_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_source_lc => _Localizer.GetString("Common_source_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_command_lc => _Localizer.GetString("Common_command_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_argument_lc => _Localizer.GetString("Common_argument_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_option_lc => _Localizer.GetString("Common_option_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_Flag => _Localizer.GetString("Common_Flag");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_commands_lc => _Localizer.GetString("Common_commands_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_arguments_lc => _Localizer.GetString("Common_arguments_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Common_options_lc => _Localizer.GetString("Common_options_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Error_File_not_found(string fullPath) => _Localizer.GetString("Error_File_not_found", fullPath);

        /// <summary>
        /// 
        /// </summary>
        public override string ValueSource_EnvVar => _Localizer.GetString("ValueSource_EnvVar");

        /// <summary>
        /// 
        /// </summary>
        public override string ValueSource_AppSetting => _Localizer.GetString("ValueSource_AppSetting");

        /// <summary>
        /// 
        /// </summary>
        public override string Command_version => _Localizer.GetString("Command_version");

        /// <summary>
        /// 
        /// </summary>
        public override string Command_version_description => _Localizer.GetString("Command_version_description");

        /// <summary>
        /// 
        /// </summary>
        public override string CommandLogger_Original_input => _Localizer.GetString("CommandLogger_Original_input");

        /// <summary>
        /// 
        /// </summary>
        public override string CommandLogger_Tool_version => _Localizer.GetString("CommandLogger_Tool_version");

        /// <summary>
        /// 
        /// </summary>
        public override string CommandLogger_DotNet_version => _Localizer.GetString("CommandLogger_DotNet_version");

        /// <summary>
        /// 
        /// </summary>
        public override string CommandLogger_OS_version => _Localizer.GetString("CommandLogger_OS_version");

        /// <summary>
        /// 
        /// </summary>
        public override string CommandLogger_Machine => _Localizer.GetString("CommandLogger_Machine");

        /// <summary>
        /// 
        /// </summary>
        public override string CommandLogger_Username => _Localizer.GetString("CommandLogger_Username");

        /// <summary>
        /// 
        /// </summary>
        public override string Exceptions_Data => _Localizer.GetString("Exceptions_Data");

        /// <summary>
        /// 
        /// </summary>
        public override string Exceptions_Properties => _Localizer.GetString("Exceptions_Properties");

        /// <summary>
        /// 
        /// </summary>
        public override string Exceptions_StackTrace => _Localizer.GetString("Exceptions_StackTrace");

        /// <summary>
        /// 
        /// </summary>
        public override string Exceptions_StackTrace_at => _Localizer.GetString("Exceptions_StackTrace_at");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public override string ParseReport_Raw_command_line(string commandLine) =>
            _Localizer.GetString("ParseReport_Raw_command_line", commandLine);

        /// <summary>
        /// 
        /// </summary>
        public override string ParseReport_root_lc => _Localizer.GetString("ParseReport_root");

        /// <summary>
        /// 
        /// </summary>
        public override string ParseDirective_Help_was_requested => _Localizer.GetString("ParseDirective_Help_was_requested");

        /// <summary>
        /// 
        /// </summary>
        public override string ParseDirective_Unable_to_map_tokens_to_arguments => _Localizer.GetString("ParseDirective_Unable_to_map_tokens_to_arguments");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeTransformationsArgName"></param>
        /// <param name="includeRawCommandLineArgName"></param>
        /// <returns></returns>
        public override string ParseDirective_Usage(string includeTransformationsArgName, string includeRawCommandLineArgName) =>
            _Localizer.GetString("ParseDirective_Usage", includeTransformationsArgName, includeRawCommandLineArgName);

        /// <summary>
        /// 
        /// </summary>
        public override string ParseDirective_parse_lc => _Localizer.GetString("ParseDirective_parse_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string ParseDirective_token_transformations_lc => _Localizer.GetString("ParseDirective_token_transformations_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string ParseDirective_from_shell_lc => _Localizer.GetString("ParseDirective_from_shell_lc");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformationName"></param>
        /// <returns></returns>
        public override string ParseDirective_after(string transformationName) =>
            _Localizer.GetString("ParseDirective_after", transformationName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformationName"></param>
        /// <returns></returns>
        public override string ParseDirective_after_no_changes(string transformationName) =>
            _Localizer.GetString("ParseDirective_after_no_changes");

        /// <summary>
        /// 
        /// </summary>
        public override string Debugger_debug_lc => _Localizer.GetString("Debugger_debug_lc");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public override string Debugger_Attach_debugger(string processId, string processName) =>
            _Localizer.GetString("Debugger_Attach_debugger", processId, processName);

        /// <summary>
        /// 
        /// </summary>
        public override string Time_time => _Localizer.GetString("Time_time");

        /// <summary>
        /// 
        /// </summary>
        public override string Prompt_Enter_once_for_new_value_twice_to_finish => _Localizer.GetString("Prompt_Enter_once_for_new_value_twice_to_finish");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        public override string Arity_is_required(string argumentName) =>
            _Localizer.GetString("Arity_is_required", argumentName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override string Arity_min_not_reached(string argumentName, string expected, string actual) =>
            _Localizer.GetString("Arity_min_not_reached", argumentName, expected, actual);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override string Arity_max_exceeded(string argumentName, string expected, string actual) =>
            _Localizer.GetString("Arity_max_exceeded", argumentName, expected, actual);

        /// <summary>
        /// 
        /// </summary>
        public override string Command_help => _Localizer.GetString("Command_help");

        /// <summary>
        /// 
        /// </summary>
        public override string Command_help_description => _Localizer.GetString(" Command_help_description");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Allowed_values => _Localizer.GetString("Help_Allowed_values");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_arg => _Localizer.GetString("Help_arg");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Arguments => _Localizer.GetString("Help_Arguments");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Commands => _Localizer.GetString("Help_Commands");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_for_more_information_about_a_command => _Localizer.GetString("Help_for_more_information_about_a_command");

        /// <summary>
        /// 
        /// </summary>
        public override string Input_inputs_lc => _Localizer.GetString("Input_inputs_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Input_stream_lc => _Localizer.GetString("Input_stream_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Input_piped_lc => _Localizer.GetString("Input_piped_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Input_prompt_lc => _Localizer.GetString("Input_prompt_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Parse_Required_command_was_not_provided =>
            _Localizer.GetString("Parse_Required_command_was_not_provided", Common_command_lc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        public override string Parse_ArgumentArity_Expected_single_value(string argumentName) =>
             _Localizer.GetString("Parse_ArgumentArity_Expected_single_value", argumentName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentToString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public override string Parse_Assigning_value_to_argument(string? argumentToString, string? defaultValue) =>
             _Localizer.GetString("Parse_Assigning_value_to_argument", argumentToString ?? string.Empty, defaultValue ?? string.Empty);

        /// <summary>
        /// 
        /// </summary>
        public override string Parse_Did_you_mean => _Localizer.GetString("Parse_Did_you_mean");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="helpCommand"></param>
        /// <returns></returns>
        public override string Parse_See_usage(string usage, string helpCommand) =>
             _Localizer.GetString("Parse_See_usage", usage, helpCommand);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public override string Parse_Is_not_a_valid(string typeName) =>
             _Localizer.GetString("Parse_Is_not_a_valid", typeName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public override string Parse_Missing_value_for_option(string optionName) =>
            _Localizer.GetString("Parse_Missing_value_for_option", Common_option_lc, optionName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="argumentType"></param>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        public override string Parse_Unrecognized_value_for(string token, string argumentType, string argumentName) =>
             _Localizer.GetString("Parse_Unrecognized_value_for", token, argumentType, argumentType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public override string Parse_Unexpected_value_for_option(string token, string optionName) =>
            _Localizer.GetString("Parse_Unexpected_value_for_option", token, Common_option_lc, optionName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="commandName"></param>
        /// <param name="suggestion"></param>
        /// <returns></returns>
        public override string Parse_Intended_command_instead_of_option(string token, string commandName, string suggestion) =>
            _Localizer.GetString("Parse_Intended_command_instead_of_option", Common_option_lc, token, commandName, Common_command_lc, suggestion);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionShortName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public override string Parse_Clubbed_options_with_values_must_be_last_option(string optionShortName, string token) =>
            _Localizer.GetString("Parse_Clubbed_options_with_values_must_be_last_option", optionShortName, Common_option_lc, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public override string Parse_Unrecognized_command_or_argument(string token) =>
            _Localizer.GetString("Parse_Unrecognized_command_or_argument", Common_option_lc, Common_argument_lc, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public override string Parse_Unrecognized_option(string optionName) =>
            _Localizer.GetString("Parse_Unrecognized_option", Common_option_lc, optionName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argumentNames"></param>
        /// <returns></returns>
        public override string Input_Piped_targetted_multiple_arguments(string argumentNames) =>
             _Localizer.GetString("Input_Piped_targetted_multiple_arguments", argumentNames);

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Multiple => _Localizer.GetString("Help_Multiple");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Options => _Localizer.GetString("Help_Options");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Options_also_available_for_subcommands => _Localizer.GetString("Help_Options_also_available_for_subcommands");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Usage => _Localizer.GetString("Help_Usage");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_usage_lc => _Localizer.GetString("Help_usage_lc");

        /// <summary>
        /// 
        /// </summary>
        public override string Help_Use => _Localizer.GetString("Help_Use");

        /// <summary>
        /// 
        /// </summary>
        public override string Type_Boolean => _Localizer.GetString("Type_Boolean");

        /// <summary>
        /// 
        /// </summary>
        public override string Type_Character => _Localizer.GetString("Type_Character");

        /// <summary>
        /// 
        /// </summary>
        public override string Type_Decimal => _Localizer.GetString("Type_Decimal");

        /// <summary>
        /// 
        /// </summary>
        public override string Type_Double => _Localizer.GetString("Type_Double");

        /// <summary>
        /// 
        /// </summary>
        public override string Type_Number => _Localizer.GetString("Type_Number");

        /// <summary>
        /// 
        /// </summary>
        public override string Type_Text => _Localizer.GetString("Type_Text");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeDisplayName"></param>
        /// <returns></returns>
        public override string Error_Value_is_not_valid_for_type(string value, string typeDisplayName) =>
            _Localizer.GetString("Error_Value_is_not_valid_for_type", value, typeDisplayName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeDisplayName"></param>
        /// <returns></returns>
        public override string Error_Failed_parsing_value_for_type(string value, string typeDisplayName) =>
            _Localizer.GetString("Error_Failed_parsing_value_for_type", value, typeDisplayName);

    }
}
