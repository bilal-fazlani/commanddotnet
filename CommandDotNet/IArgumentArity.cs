namespace CommandDotNet
{
    /// <summary>Arity describes how many values are allowed for an argument</summary>
    public interface IArgumentArity
    {
        int MinimumNumberOfValues { get; }

        int MaximumNumberOfValues { get; }
    }
}