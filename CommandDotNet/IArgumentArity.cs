namespace CommandDotNet
{
    /// <summary>Arity describes how many values a user can or must provide for an argument</summary>
    public interface IArgumentArity
    {
        /// <summary>The minimum number of values the user must provide</summary>
        int Minimum { get; }

        /// <summary>The maximum number of values the user must provide</summary>
        int Maximum { get; }
    }
}