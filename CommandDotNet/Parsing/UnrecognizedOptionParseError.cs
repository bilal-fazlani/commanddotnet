using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// <see cref="Token"/> is not a valid command
    /// and there is no available operand to assign the value to.
    /// </summary>
    public class UnrecognizedOptionParseError : UnrecognizedArgumentParseError
    {
        public UnrecognizedOptionParseError(Command command, Token token, string optionPrefix, string? message = null)
        : base(command, token, optionPrefix, message ?? Resources.A.Parse_Unrecognized_option(token.RawValue))
        {
        }
    }
}