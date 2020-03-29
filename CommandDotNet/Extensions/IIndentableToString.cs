namespace CommandDotNet.Extensions
{
    public interface IIndentableToString
    {
        string ToString(Indent indent);
    }
}