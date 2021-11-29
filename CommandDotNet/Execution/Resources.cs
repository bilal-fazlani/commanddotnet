// ReSharper disable CheckNamespace
namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Arity_is_required(string argumentName) => $"{argumentName} is required";
        public virtual string Arity_min_not_reached(string argumentName, string expected, string actual) => 
            $"{argumentName} requires at least {expected} values but {actual} were provided.";
        public virtual string Arity_max_exceeded(string argumentName, string expected, string actual) =>
            $"{argumentName} can have no more than {expected} values but {actual} were provided.";
    }
}