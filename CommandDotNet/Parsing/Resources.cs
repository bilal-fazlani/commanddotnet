﻿// ReSharper disable CheckNamespace
namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Input_inputs_lc => "inputs";
        public virtual string Input_stream_lc => "stream";
        public virtual string Input_piped_lc => "piped";
        public virtual string Input_prompt_lc => "prompt";

        public virtual string Error_ArgumentArity_Expected_single_value(string argumentName) => 
            $"{argumentName} accepts only a single value but multiple values were provided";

        public virtual string Error_assigning_value_to_argument(string argumentToString, string? defaultValue) => 
            $"Failure assigning value to {argumentToString}. Value={defaultValue}";
    }
}