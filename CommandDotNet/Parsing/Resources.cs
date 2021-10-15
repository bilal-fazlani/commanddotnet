// ReSharper disable CheckNamespace
namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Input_piped_lc => "piped";
        public virtual string Input_prompt_lc => "prompt";

        public string Error_ArgumentArity_Expected_single_value(string argumentName) => 
            $"{argumentName} accepts only a single value but multiple values were provided";

        public string? Error_assigning_value_to_argument(IArgument argument, object defaultValue) => 
            $"Failure assigning value to {argument}. Value={defaultValue}";
    }
}