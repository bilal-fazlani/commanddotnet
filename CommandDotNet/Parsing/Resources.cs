// ReSharper disable CheckNamespace

using System.Reflection;

namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Input_inputs_lc => "inputs";
        public virtual string Input_stream_lc => "stream";
        public virtual string Input_piped_lc => "piped";
        public virtual string Input_prompt_lc => "prompt";

        public virtual string Parse_ArgumentArity_Expected_single_value(string argumentName) =>
            $"{argumentName} accepts only a single value but multiple values were provided";

        public virtual string Parse_assigning_value_to_argument(string argumentToString, string? defaultValue) =>
            $"Failure assigning value to {argumentToString}. Value={defaultValue}";

        public virtual string Parse_Did_you_mean => "Did you mean ...";

        public virtual string Parse_See_usage(string usage, string helpCommand) =>
            $"See '{usage} --{helpCommand}'";

        public virtual string Parse_is_not_a_valid(string typeName) =>
            $"is not a valid {typeName}";

        public virtual string Parse_Missing_value_for_option(string optionName) =>
            $"Missing value for option '{optionName}'";

        public virtual string Parse_Unrecognized_value_for(string token, string argumentType, string argumentName) =>
            $"Unrecognized value '{token}' for {argumentType}: {argumentName}";

        public virtual string Parse_unexpected_value_for_option(string token, string optionName) =>
            $"Unexpected value '{token}' for option '{optionName}'";
    }
}