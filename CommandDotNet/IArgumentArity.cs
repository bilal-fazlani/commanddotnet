namespace CommandDotNet
{
    public interface IArgumentArity
    {
        int MinimumNumberOfValues { get; }

        int MaximumNumberOfValues { get; }
    }
}