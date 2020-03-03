namespace CommandDotNet.Extensions
{
    internal static class CharExtensions
    {
        internal static bool IsNullOrWhitespace(this char? c)
        {
            return !c.HasValue || char.IsWhiteSpace(c.Value);
        }
    }
}