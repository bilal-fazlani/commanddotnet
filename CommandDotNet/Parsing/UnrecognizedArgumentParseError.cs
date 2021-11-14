using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// <see cref="Token"/> is not a valid command
    /// and there is no available operand to assign the value to.
    /// </summary>
    public class UnrecognizedArgumentParseError: IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public Token Token { get; }
        public string? OptionPrefix { get; }

        public UnrecognizedArgumentParseError(Command command, Token token, string? optionPrefix, string message)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            OptionPrefix = optionPrefix;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}