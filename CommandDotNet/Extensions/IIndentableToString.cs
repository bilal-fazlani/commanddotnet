namespace CommandDotNet.Extensions
{
    public interface IIndentableToString
    {
        string ToString(string indent, int depth = 0);
    }
}