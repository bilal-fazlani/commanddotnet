// ReSharper disable CheckNamespace

using System.Collections.Generic;
using static System.Environment;

namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Input_inputs_lc => "inputs";
        public virtual string Input_stream_lc => "stream";
        public virtual string Input_piped_lc => "piped";
        public virtual string Input_prompt_lc => "prompt";

        public virtual string Parse_Required_command_was_not_provided =>
            $"Required {Common_command_lc} was not provided";

        public virtual string Parse_ArgumentArity_Expected_single_value(string argumentName) =>
            $"{argumentName} accepts only a single value but multiple values were provided";

        public virtual string Parse_Assigning_value_to_argument(string? argumentToString, string? defaultValue) =>
            $"Failure assigning value to {argumentToString}. Value={defaultValue}";

        public virtual string Parse_Did_you_mean => "Did you mean ...";

        public virtual string Parse_See_usage(string usage, string helpCommand) =>
            $"See '{usage} --{helpCommand}'";

        public virtual string Parse_Is_not_a_valid(string typeName) =>
            $"is not a valid {typeName}";

        public virtual string Parse_Missing_value_for_option(string optionName) =>
            $"Missing value for {Common_option_lc} '{optionName}'";

        public virtual string Parse_Unrecognized_value_for(string token, string argumentType, string argumentName) =>
            $"Unrecognized value '{token}' for {argumentType}: {argumentName}";

        public virtual string Parse_Unexpected_value_for_option(string token, string optionName) =>
            $"Unexpected value '{token}' for {Common_option_lc} '{optionName}'";

        public virtual string Parse_Intended_command_instead_of_option(string token, string commandName, string suggestion) =>
            $"Unrecognized {Common_option_lc} '{token}'{NewLine}" +
            $"If you intended to use the '{commandName}' {Common_command_lc}, " +
            $"try again with the following{NewLine}{NewLine}{suggestion}";

        public virtual string Parse_Clubbed_options_with_values_must_be_last_option(string optionShortName, string token) => 
            $"'{optionShortName}' expects a value so it must be the last {Common_option_lc} specified in '{token}'";

        public virtual string Parse_Unrecognized_command_or_argument(string token) =>
            $"Unrecognized {Common_command_lc} or {Common_argument_lc} '{token}'";
        public virtual string Parse_Unrecognized_option(string optionName) =>
            $"Unrecognized {Common_option_lc} '{optionName}'";

        public virtual string Input_Piped_targetted_multiple_arguments(string argumentNames) =>
            $"Piped input can only target a single argument, but the following were targeted: {argumentNames}";
    }
}