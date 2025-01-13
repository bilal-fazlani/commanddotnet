// this file generated by ResourceGenerators.RegenerateProxyClasses

using System;
using JetBrains.Annotations;

namespace CommandDotNet
{
    // this class generated by ResourcesDef.GenerateProxyClass
    [PublicAPI]
    public class ResourcesProxy(Func<string, string?> localize, bool memberNameAsKey = false) : Resources
    {
        private readonly Func<string, string?> _localize = localize ?? throw new ArgumentNullException(nameof(localize));

        private static string? Format(string? value, params object?[] args) =>
            value is null ? null : string.Format(value, args);

        public override string ValueSource_EnvVar =>
            _localize(memberNameAsKey 
                ? "ValueSource_EnvVar"
                : base.ValueSource_EnvVar)
            ?? base.ValueSource_EnvVar;

        public override string ValueSource_AppSetting =>
            _localize(memberNameAsKey 
                ? "ValueSource_AppSetting"
                : base.ValueSource_AppSetting)
            ?? base.ValueSource_AppSetting;

        public override string Command_version =>
            _localize(memberNameAsKey 
                ? "Command_version"
                : base.Command_version)
            ?? base.Command_version;

        public override string Command_version_description =>
            _localize(memberNameAsKey 
                ? "Command_version_description"
                : base.Command_version_description)
            ?? base.Command_version_description;

        public override string CommandLogger_Original_input =>
            _localize(memberNameAsKey 
                ? "CommandLogger_Original_input"
                : base.CommandLogger_Original_input)
            ?? base.CommandLogger_Original_input;

        public override string CommandLogger_Tool_version =>
            _localize(memberNameAsKey 
                ? "CommandLogger_Tool_version"
                : base.CommandLogger_Tool_version)
            ?? base.CommandLogger_Tool_version;

        public override string CommandLogger_DotNet_version =>
            _localize(memberNameAsKey 
                ? "CommandLogger_DotNet_version"
                : base.CommandLogger_DotNet_version)
            ?? base.CommandLogger_DotNet_version;

        public override string CommandLogger_OS_version =>
            _localize(memberNameAsKey 
                ? "CommandLogger_OS_version"
                : base.CommandLogger_OS_version)
            ?? base.CommandLogger_OS_version;

        public override string CommandLogger_Machine =>
            _localize(memberNameAsKey 
                ? "CommandLogger_Machine"
                : base.CommandLogger_Machine)
            ?? base.CommandLogger_Machine;

        public override string CommandLogger_Username =>
            _localize(memberNameAsKey 
                ? "CommandLogger_Username"
                : base.CommandLogger_Username)
            ?? base.CommandLogger_Username;

        public override string Exceptions_Data =>
            _localize(memberNameAsKey 
                ? "Exceptions_Data"
                : base.Exceptions_Data)
            ?? base.Exceptions_Data;

        public override string Exceptions_Properties =>
            _localize(memberNameAsKey 
                ? "Exceptions_Properties"
                : base.Exceptions_Properties)
            ?? base.Exceptions_Properties;

        public override string Exceptions_StackTrace =>
            _localize(memberNameAsKey 
                ? "Exceptions_StackTrace"
                : base.Exceptions_StackTrace)
            ?? base.Exceptions_StackTrace;

        public override string Exceptions_StackTrace_at =>
            _localize(memberNameAsKey 
                ? "Exceptions_StackTrace_at"
                : base.Exceptions_StackTrace_at)
            ?? base.Exceptions_StackTrace_at;

        public override string ParseReport_root_lc =>
            _localize(memberNameAsKey 
                ? "ParseReport_root_lc"
                : base.ParseReport_root_lc)
            ?? base.ParseReport_root_lc;

        public override string ParseDirective_Help_was_requested =>
            _localize(memberNameAsKey 
                ? "ParseDirective_Help_was_requested"
                : base.ParseDirective_Help_was_requested)
            ?? base.ParseDirective_Help_was_requested;

        public override string ParseDirective_Unable_to_map_tokens_to_arguments =>
            _localize(memberNameAsKey 
                ? "ParseDirective_Unable_to_map_tokens_to_arguments"
                : base.ParseDirective_Unable_to_map_tokens_to_arguments)
            ?? base.ParseDirective_Unable_to_map_tokens_to_arguments;

        public override string ParseDirective_parse_lc =>
            _localize(memberNameAsKey 
                ? "ParseDirective_parse_lc"
                : base.ParseDirective_parse_lc)
            ?? base.ParseDirective_parse_lc;

        public override string ParseDirective_token_transformations_lc =>
            _localize(memberNameAsKey 
                ? "ParseDirective_token_transformations_lc"
                : base.ParseDirective_token_transformations_lc)
            ?? base.ParseDirective_token_transformations_lc;

        public override string ParseDirective_from_shell_lc =>
            _localize(memberNameAsKey 
                ? "ParseDirective_from_shell_lc"
                : base.ParseDirective_from_shell_lc)
            ?? base.ParseDirective_from_shell_lc;

        public override string Debugger_debug_lc =>
            _localize(memberNameAsKey 
                ? "Debugger_debug_lc"
                : base.Debugger_debug_lc)
            ?? base.Debugger_debug_lc;

        public override string Time_time =>
            _localize(memberNameAsKey 
                ? "Time_time"
                : base.Time_time)
            ?? base.Time_time;

        public override string Command_help =>
            _localize(memberNameAsKey 
                ? "Command_help"
                : base.Command_help)
            ?? base.Command_help;

        public override string Command_help_description =>
            _localize(memberNameAsKey 
                ? "Command_help_description"
                : base.Command_help_description)
            ?? base.Command_help_description;

        public override string Help_Allowed_values =>
            _localize(memberNameAsKey 
                ? "Help_Allowed_values"
                : base.Help_Allowed_values)
            ?? base.Help_Allowed_values;

        public override string Help_arg =>
            _localize(memberNameAsKey 
                ? "Help_arg"
                : base.Help_arg)
            ?? base.Help_arg;

        public override string Help_Arguments =>
            _localize(memberNameAsKey 
                ? "Help_Arguments"
                : base.Help_Arguments)
            ?? base.Help_Arguments;

        public override string Help_Commands =>
            _localize(memberNameAsKey 
                ? "Help_Commands"
                : base.Help_Commands)
            ?? base.Help_Commands;

        public override string Help_for_more_information_about_a_command =>
            _localize(memberNameAsKey 
                ? "Help_for_more_information_about_a_command"
                : base.Help_for_more_information_about_a_command)
            ?? base.Help_for_more_information_about_a_command;

        public override string Help_Multiple =>
            _localize(memberNameAsKey 
                ? "Help_Multiple"
                : base.Help_Multiple)
            ?? base.Help_Multiple;

        public override string Help_Options =>
            _localize(memberNameAsKey 
                ? "Help_Options"
                : base.Help_Options)
            ?? base.Help_Options;

        public override string Help_Options_also_available_for_subcommands =>
            _localize(memberNameAsKey 
                ? "Help_Options_also_available_for_subcommands"
                : base.Help_Options_also_available_for_subcommands)
            ?? base.Help_Options_also_available_for_subcommands;

        public override string Help_Usage =>
            _localize(memberNameAsKey 
                ? "Help_Usage"
                : base.Help_Usage)
            ?? base.Help_Usage;

        public override string Help_usage_lc =>
            _localize(memberNameAsKey 
                ? "Help_usage_lc"
                : base.Help_usage_lc)
            ?? base.Help_usage_lc;

        public override string Help_Use =>
            _localize(memberNameAsKey 
                ? "Help_Use"
                : base.Help_Use)
            ?? base.Help_Use;

        public override string Input_inputs_lc =>
            _localize(memberNameAsKey 
                ? "Input_inputs_lc"
                : base.Input_inputs_lc)
            ?? base.Input_inputs_lc;

        public override string Input_stream_lc =>
            _localize(memberNameAsKey 
                ? "Input_stream_lc"
                : base.Input_stream_lc)
            ?? base.Input_stream_lc;

        public override string Input_piped_lc =>
            _localize(memberNameAsKey 
                ? "Input_piped_lc"
                : base.Input_piped_lc)
            ?? base.Input_piped_lc;

        public override string Input_prompt_lc =>
            _localize(memberNameAsKey 
                ? "Input_prompt_lc"
                : base.Input_prompt_lc)
            ?? base.Input_prompt_lc;

        public override string Parse_Required_command_was_not_provided =>
            _localize(memberNameAsKey 
                ? "Parse_Required_command_was_not_provided"
                : base.Parse_Required_command_was_not_provided)
            ?? base.Parse_Required_command_was_not_provided;

        public override string Parse_Did_you_mean =>
            _localize(memberNameAsKey 
                ? "Parse_Did_you_mean"
                : base.Parse_Did_you_mean)
            ?? base.Parse_Did_you_mean;

        public override string Prompt_Enter_once_for_new_value_twice_to_finish =>
            _localize(memberNameAsKey 
                ? "Prompt_Enter_once_for_new_value_twice_to_finish"
                : base.Prompt_Enter_once_for_new_value_twice_to_finish)
            ?? base.Prompt_Enter_once_for_new_value_twice_to_finish;

        public override string Common_default_lc =>
            _localize(memberNameAsKey 
                ? "Common_default_lc"
                : base.Common_default_lc)
            ?? base.Common_default_lc;

        public override string Common_from_lc =>
            _localize(memberNameAsKey 
                ? "Common_from_lc"
                : base.Common_from_lc)
            ?? base.Common_from_lc;

        public override string Common_key_lc =>
            _localize(memberNameAsKey 
                ? "Common_key_lc"
                : base.Common_key_lc)
            ?? base.Common_key_lc;

        public override string Common_value_lc =>
            _localize(memberNameAsKey 
                ? "Common_value_lc"
                : base.Common_value_lc)
            ?? base.Common_value_lc;

        public override string Common_source_lc =>
            _localize(memberNameAsKey 
                ? "Common_source_lc"
                : base.Common_source_lc)
            ?? base.Common_source_lc;

        public override string Common_command_lc =>
            _localize(memberNameAsKey 
                ? "Common_command_lc"
                : base.Common_command_lc)
            ?? base.Common_command_lc;

        public override string Common_argument_lc =>
            _localize(memberNameAsKey 
                ? "Common_argument_lc"
                : base.Common_argument_lc)
            ?? base.Common_argument_lc;

        public override string Common_option_lc =>
            _localize(memberNameAsKey 
                ? "Common_option_lc"
                : base.Common_option_lc)
            ?? base.Common_option_lc;

        public override string Common_Flag =>
            _localize(memberNameAsKey 
                ? "Common_Flag"
                : base.Common_Flag)
            ?? base.Common_Flag;

        public override string Common_commands_lc =>
            _localize(memberNameAsKey 
                ? "Common_commands_lc"
                : base.Common_commands_lc)
            ?? base.Common_commands_lc;

        public override string Common_arguments_lc =>
            _localize(memberNameAsKey 
                ? "Common_arguments_lc"
                : base.Common_arguments_lc)
            ?? base.Common_arguments_lc;

        public override string Common_options_lc =>
            _localize(memberNameAsKey 
                ? "Common_options_lc"
                : base.Common_options_lc)
            ?? base.Common_options_lc;

        public override string Type_Boolean =>
            _localize(memberNameAsKey 
                ? "Type_Boolean"
                : base.Type_Boolean)
            ?? base.Type_Boolean;

        public override string Type_Character =>
            _localize(memberNameAsKey 
                ? "Type_Character"
                : base.Type_Character)
            ?? base.Type_Character;

        public override string Type_Decimal =>
            _localize(memberNameAsKey 
                ? "Type_Decimal"
                : base.Type_Decimal)
            ?? base.Type_Decimal;

        public override string Type_Double =>
            _localize(memberNameAsKey 
                ? "Type_Double"
                : base.Type_Double)
            ?? base.Type_Double;

        public override string Type_Number =>
            _localize(memberNameAsKey 
                ? "Type_Number"
                : base.Type_Number)
            ?? base.Type_Number;

        public override string Type_Text =>
            _localize(memberNameAsKey 
                ? "Type_Text"
                : base.Type_Text)
            ?? base.Type_Text;

        public override string ParseReport_Raw_command_line(string commandLine) =>
            Format(_localize(memberNameAsKey 
                ? "ParseReport_Raw_command_line"
                : base.ParseReport_Raw_command_line("{0}")),
                commandLine)
            ?? base.ParseReport_Raw_command_line(commandLine);

        public override string ParseDirective_Usage(string includeTransformationsArgName, string includeRawCommandLineArgName) =>
            Format(_localize(memberNameAsKey 
                ? "ParseDirective_Usage"
                : base.ParseDirective_Usage("{0}", "{1}")),
                includeTransformationsArgName, includeRawCommandLineArgName)
            ?? base.ParseDirective_Usage(includeTransformationsArgName, includeRawCommandLineArgName);

        public override string ParseDirective_after(string transformationName) =>
            Format(_localize(memberNameAsKey 
                ? "ParseDirective_after"
                : base.ParseDirective_after("{0}")),
                transformationName)
            ?? base.ParseDirective_after(transformationName);

        public override string ParseDirective_after_no_changes(string transformationName) =>
            Format(_localize(memberNameAsKey 
                ? "ParseDirective_after_no_changes"
                : base.ParseDirective_after_no_changes("{0}")),
                transformationName)
            ?? base.ParseDirective_after_no_changes(transformationName);

        public override string Debugger_Attach_debugger(string processId, string processName) =>
            Format(_localize(memberNameAsKey 
                ? "Debugger_Attach_debugger"
                : base.Debugger_Attach_debugger("{0}", "{1}")),
                processId, processName)
            ?? base.Debugger_Attach_debugger(processId, processName);

        public override string Arity_is_required(string argumentName) =>
            Format(_localize(memberNameAsKey 
                ? "Arity_is_required"
                : base.Arity_is_required("{0}")),
                argumentName)
            ?? base.Arity_is_required(argumentName);

        public override string Arity_min_not_reached(string argumentName, string expected, string actual) =>
            Format(_localize(memberNameAsKey 
                ? "Arity_min_not_reached"
                : base.Arity_min_not_reached("{0}", "{1}", "{2}")),
                argumentName, expected, actual)
            ?? base.Arity_min_not_reached(argumentName, expected, actual);

        public override string Arity_max_exceeded(string argumentName, string expected, string actual) =>
            Format(_localize(memberNameAsKey 
                ? "Arity_max_exceeded"
                : base.Arity_max_exceeded("{0}", "{1}", "{2}")),
                argumentName, expected, actual)
            ?? base.Arity_max_exceeded(argumentName, expected, actual);

        public override string Parse_ArgumentArity_Expected_single_value(string argumentName) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_ArgumentArity_Expected_single_value"
                : base.Parse_ArgumentArity_Expected_single_value("{0}")),
                argumentName)
            ?? base.Parse_ArgumentArity_Expected_single_value(argumentName);

        public override string Parse_Assigning_value_to_argument(string? argumentToString, string? defaultValue) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Assigning_value_to_argument"
                : base.Parse_Assigning_value_to_argument("{0}", "{1}")),
                argumentToString, defaultValue)
            ?? base.Parse_Assigning_value_to_argument(argumentToString, defaultValue);

        public override string Parse_See_usage(string usage, string helpCommand) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_See_usage"
                : base.Parse_See_usage("{0}", "{1}")),
                usage, helpCommand)
            ?? base.Parse_See_usage(usage, helpCommand);

        public override string Parse_Is_not_a_valid(string typeName) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Is_not_a_valid"
                : base.Parse_Is_not_a_valid("{0}")),
                typeName)
            ?? base.Parse_Is_not_a_valid(typeName);

        public override string Parse_Missing_value_for_option(string optionName) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Missing_value_for_option"
                : base.Parse_Missing_value_for_option("{0}")),
                optionName)
            ?? base.Parse_Missing_value_for_option(optionName);

        public override string Parse_Unrecognized_value_for(string token, string argumentType, string argumentName) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Unrecognized_value_for"
                : base.Parse_Unrecognized_value_for("{0}", "{1}", "{2}")),
                token, argumentType, argumentName)
            ?? base.Parse_Unrecognized_value_for(token, argumentType, argumentName);

        public override string Parse_Unexpected_value_for_option(string token, string optionName) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Unexpected_value_for_option"
                : base.Parse_Unexpected_value_for_option("{0}", "{1}")),
                token, optionName)
            ?? base.Parse_Unexpected_value_for_option(token, optionName);

        public override string Parse_Intended_command_instead_of_option(string token, string commandName, string suggestion) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Intended_command_instead_of_option"
                : base.Parse_Intended_command_instead_of_option("{0}", "{1}", "{2}")),
                token, commandName, suggestion)
            ?? base.Parse_Intended_command_instead_of_option(token, commandName, suggestion);

        public override string Parse_Clubbed_options_with_values_must_be_last_option(string optionShortName, string token) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Clubbed_options_with_values_must_be_last_option"
                : base.Parse_Clubbed_options_with_values_must_be_last_option("{0}", "{1}")),
                optionShortName, token)
            ?? base.Parse_Clubbed_options_with_values_must_be_last_option(optionShortName, token);

        public override string Parse_Unrecognized_command_or_argument(string token) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Unrecognized_command_or_argument"
                : base.Parse_Unrecognized_command_or_argument("{0}")),
                token)
            ?? base.Parse_Unrecognized_command_or_argument(token);

        public override string Parse_Unrecognized_option(string optionName) =>
            Format(_localize(memberNameAsKey 
                ? "Parse_Unrecognized_option"
                : base.Parse_Unrecognized_option("{0}")),
                optionName)
            ?? base.Parse_Unrecognized_option(optionName);

        public override string Input_Piped_targetted_multiple_arguments(string argumentNames) =>
            Format(_localize(memberNameAsKey 
                ? "Input_Piped_targetted_multiple_arguments"
                : base.Input_Piped_targetted_multiple_arguments("{0}")),
                argumentNames)
            ?? base.Input_Piped_targetted_multiple_arguments(argumentNames);

        public override string Error_File_not_found(string fullPath) =>
            Format(_localize(memberNameAsKey 
                ? "Error_File_not_found"
                : base.Error_File_not_found("{0}")),
                fullPath)
            ?? base.Error_File_not_found(fullPath);

        public override string Error_Value_is_not_valid_for_type(string value, string typeDisplayName) =>
            Format(_localize(memberNameAsKey 
                ? "Error_Value_is_not_valid_for_type"
                : base.Error_Value_is_not_valid_for_type("{0}", "{1}")),
                value, typeDisplayName)
            ?? base.Error_Value_is_not_valid_for_type(value, typeDisplayName);

        public override string Error_Failed_parsing_value_for_type(string value, string typeDisplayName) =>
            Format(_localize(memberNameAsKey 
                ? "Error_Failed_parsing_value_for_type"
                : base.Error_Failed_parsing_value_for_type("{0}", "{1}")),
                value, typeDisplayName)
            ?? base.Error_Failed_parsing_value_for_type(value, typeDisplayName);

    }
}