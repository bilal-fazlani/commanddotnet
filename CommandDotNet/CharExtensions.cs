namespace CommandDotNet
{
    internal static class CharExtensions
    {
        internal static bool IsAlphaNumeric(this char c) => char.IsLetterOrDigit(c);
    }
}