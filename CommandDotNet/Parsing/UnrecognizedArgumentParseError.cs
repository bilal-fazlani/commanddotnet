using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// When <see cref="TokenType"/> is <see cref="TokenType.Option"/>,
    /// <see cref="Token"/> is not a valid option name<br/>
    /// Otherwise, <see cref="Token"/> is not a valid command name
    /// and there is no available operand to assign the value to.
    /// </summary>
    public class UnrecognizedArgumentParseError: IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public Token Token { get; }

        public UnrecognizedArgumentParseError(Command command, Token token, string message)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}